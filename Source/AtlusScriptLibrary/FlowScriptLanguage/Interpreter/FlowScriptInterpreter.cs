﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AtlusScriptLibrary.FlowScriptLanguage.Interpreter;

public class FlowScriptInterpreter
{
    private static readonly Func<FlowScriptInterpreter, bool>[] sOpcodeHandlers =
    {
        PUSHI, PUSHF, PUSHIX, PUSHIF, PUSHREG,
        POPIX, POPFX, PROC, COMM, END, JUMP,
        CALL, RUN, GOTO, ADD, SUB,
        MUL, DIV, MINUS, NOT, OR,
        AND, EQ, NEQ, S, L,
        SE, LE, IF, PUSHIS, PUSHLIX,
        PUSHLFX, POPLIX, POPLFX, PUSHSTR, POPREG
    };

    // Static
    public static readonly int[] GlobalIntVariablePool = new int[256];
    public static readonly float[] GlobalFloatVariablePool = new float[256];

    // Instance
    private readonly FlowScript mScript;
    private int mProcedureIndex;
    private int mInstructionIndex;
    private readonly Stack<StackValue> mStack;
    private StackValue mREGValue;
    private readonly int[] mLocalIntVariablePool;
    private readonly float[] mLocalFloatVariablePool;

    private int ProcedureIndex
    {
        get => mProcedureIndex;
        set => mProcedureIndex = value;
    }

    private Procedure Procedure => mScript.Procedures[mProcedureIndex];

    private int InstructionIndex
    {
        get => mInstructionIndex;
        set => mInstructionIndex = value;
    }

    private Instruction Instruction => Procedure.Instructions[mInstructionIndex];

    public TextWriter TextOutput { get; set; }

    public int[] CountValues { get; } = new int[1024];

    public int[] LocalIntVariables => mLocalIntVariablePool;

    public float[] LocalFloatVariables => mLocalFloatVariablePool;

    public FlowScriptInterpreter(FlowScript flowScript)
    {
        mScript = flowScript;
        mProcedureIndex = 0;
        mInstructionIndex = 0;
        mStack = new Stack<StackValue>();
        mREGValue = new StackValue(StackValueKind.Int, 0);
        mLocalIntVariablePool = new int[CalculateLocalCount(mScript, true)];
        mLocalFloatVariablePool = new float[CalculateLocalCount(mScript, false)];
        TextOutput = Console.Out;
    }

    private static int CalculateLocalCount(FlowScript script, bool isInt)
    {
        var highestIndex = script.Procedures.SelectMany(x => x.Instructions)
                                 .Where(x => isInt
                                             ? x.Opcode == Opcode.PUSHLIX || x.Opcode == Opcode.POPLIX
                                             : x.Opcode == Opcode.PUSHLFX || x.Opcode == Opcode.POPLFX)
                                 .Aggregate(-1, (current, instruction) => Math.Max(current, instruction.Operand.UInt16Value));

        return highestIndex + 1;
    }

    public void Run()
    {
        while (Step()) ;

        if (mStack.Count > 1)
            throw new StackInbalanceException();
    }

    public bool Step()
    {
        // Save current instruction index for later
        var prevProcedureIndex = ProcedureIndex;
        var prevInstructionIndex = InstructionIndex;

        // Invoke handler
        var continueFlag = sOpcodeHandlers[(int)Instruction.Opcode](this);
        if (!continueFlag)
        {
            // done
            return false;
        }

        // Only increment instruction index if opcode didn't modify it
        if (ProcedureIndex == prevProcedureIndex && InstructionIndex == prevInstructionIndex)
            ++InstructionIndex;

        return mInstructionIndex < Procedure.Instructions.Count;
    }

    private bool IsStackEmpty() => mStack.Count == 0;

    // Push
    private void PushValue(StackValue stackValue)
    {
        mStack.Push(stackValue);
    }

    private void PushValue(StackValueKind kind, object value)
    {
        PushValue(new StackValue(kind, value));
    }

    private void PushValue(int value)
    {
        PushValue(StackValueKind.Int, value);
    }

    private void PushValue(bool value)
    {
        PushValue(StackValueKind.Int, value ? 1 : 0);
    }

    private void PushValue(float value)
    {
        PushValue(StackValueKind.Float, value);
    }

    private void PushValue(string value)
    {
        PushValue(StackValueKind.String, value);
    }

    // Pop
    private StackValue PopValue()
    {
        if (IsStackEmpty())
            throw new StackUnderflowException();

        return mStack.Pop();
    }

    private StackValue PopArithmicValue()
    {
        var value = PopValue();

        if (value.Kind == StackValueKind.String || value.Kind == StackValueKind.ReturnIndex)
        {
            throw new InvalidStackValueTypeException($"Attempted to perform arithmic on a {value.Kind} value");
        }

        return value;
    }

    private int PopIntValue()
    {
        var value = PopValue();

        switch (value.Kind)
        {
            case StackValueKind.Int:
                return (int)value.Value;

            case StackValueKind.Float:
                return (int)(float)value.Value;

            case StackValueKind.GlobalIntVariable:
                return GlobalIntVariablePool[(int)value.Value];

            case StackValueKind.GlobalFloatVariable:
                return (int)GlobalFloatVariablePool[(int)value.Value];

            default:
                throw new InvalidStackValueTypeException(StackValueKind.Int, value.Kind);
        }
    }

    private float PopFloatValue()
    {
        var value = PopValue();

        switch (value.Kind)
        {
            case StackValueKind.Int:
                return (float)(int)value.Value;

            case StackValueKind.Float:
                return (float)value.Value;

            case StackValueKind.GlobalIntVariable:
                return (float)GlobalIntVariablePool[(int)value.Value];

            case StackValueKind.GlobalFloatVariable:
                return GlobalFloatVariablePool[(int)value.Value];

            default:
                throw new InvalidStackValueTypeException(StackValueKind.Float, value.Kind);
        }
    }

    private string PopStringValue()
    {
        var value = PopValue();
        if (value.Kind != StackValueKind.String)
        {
            throw new InvalidStackValueTypeException(StackValueKind.String, value.Kind);
        }

        return (string)value.Value;
    }

    private bool PopBooleanValue()
    {
        return PopIntValue() != 0;
    }

    // COMM pop/push stuff
    private void SetReturnValue(StackValue stackValue)
    {
        mREGValue = stackValue;
    }

    private void SetReturnValue(StackValueKind kind, object value)
    {
        SetReturnValue(new StackValue(kind, value));
    }

    private void SetIntReturnValue(int value)
    {
        SetReturnValue(StackValueKind.Int, value);
    }

    private void SetBoolReturnValue(bool value)
    {
        SetIntReturnValue(value ? 1 : 0);
    }

    private void SetFloatReturnValue(float value)
    {
        SetReturnValue(StackValueKind.Float, value);
    }

    // Opcode handlers
    private static bool PUSHI(FlowScriptInterpreter instance)
    {
        instance.PushValue(instance.Instruction.Operand.UInt32Value);
        return true;
    }

    private static bool PUSHF(FlowScriptInterpreter instance)
    {
        instance.PushValue(instance.Instruction.Operand.SingleValue);
        return true;
    }

    private static bool PUSHIX(FlowScriptInterpreter instance)
    {
        instance.PushValue(GlobalIntVariablePool[instance.Instruction.Operand.UInt16Value]);
        return true;
    }

    private static bool PUSHIF(FlowScriptInterpreter instance)
    {
        instance.PushValue(GlobalFloatVariablePool[instance.Instruction.Operand.UInt16Value]);
        return true;
    }

    private static bool PUSHREG(FlowScriptInterpreter instance)
    {
        instance.PushValue(instance.mREGValue);
        return true;
    }

    private static bool POPIX(FlowScriptInterpreter instance)
    {
        GlobalIntVariablePool[instance.Instruction.Operand.UInt16Value] = instance.PopIntValue();
        return true;
    }

    private static bool POPFX(FlowScriptInterpreter instance)
    {
        GlobalFloatVariablePool[instance.Instruction.Operand.UInt16Value] = instance.PopFloatValue();
        return true;
    }

    private static bool PROC(FlowScriptInterpreter instance)
    {
        // Nothing to do?
        return true;
    }

    private static bool COMM(FlowScriptInterpreter instance)
    {
        var index = instance.Instruction.Operand.UInt16Value;

        // TODO
        switch (index)
        {
            // PUT
            case 0x0002:
                instance.TextOutput?.Write(instance.PopIntValue() + "\n");
                break;

            // PUTS
            case 0x0003:
                {
                    var format = instance.PopStringValue();

                    for (int i = 0; i < format.Length; i++)
                    {
                        var c = format[i];

                        if (c == '%' && ++i < format.Length)
                        {
                            var next = format[i];

                            switch (next)
                            {
                                case 'c':
                                case 's':
                                    instance.TextOutput?.Write(instance.PopStringValue());
                                    break;

                                case 'd':
                                case 'i':
                                case 'o':
                                case 'x':
                                case 'X':
                                case 'u':
                                    instance.TextOutput?.Write(instance.PopIntValue());
                                    break;

                                case 'f':
                                case 'F':
                                case 'e':
                                case 'E':
                                case 'a':
                                case 'A':
                                case 'g':
                                case 'G':
                                    instance.TextOutput?.Write(instance.PopFloatValue().ToString(CultureInfo.InvariantCulture));
                                    break;
                            }
                        }
                        else
                        {
                            instance.TextOutput?.Write(c);
                        }
                    }

                    instance.TextOutput?.Write('\n');
                    break;
                }

            // PUTF
            case 0x0004:
                instance.TextOutput?.Write(instance.PopFloatValue() + "\n");
                break;

            // SIN
            case 0x00B6:
                instance.SetFloatReturnValue((float)Math.Sin(instance.PopFloatValue()));
                break;

            // COS
            case 0x00B7:
                instance.SetFloatReturnValue((float)Math.Cos(instance.PopFloatValue()));
                break;

            // GET_COUNT
            case 0x000f:
                instance.SetIntReturnValue(instance.CountValues[instance.PopIntValue()]);
                break;

            // SET_COUNT
            case 0x0010:
                instance.CountValues[instance.PopIntValue()] = instance.PopIntValue();
                break;

            default:
                throw new NotImplementedException($"COMM function: {index:X8}");
        }

        return true;
    }

    private static bool END(FlowScriptInterpreter instance)
    {
        // Nothing to return to if stack is empty
        if (instance.IsStackEmpty())
            return false;

        // Set procedure & instruction index to the one we stored during CALL
        var returnIndexValue = instance.PopValue();
        if (returnIndexValue.Kind != StackValueKind.ReturnIndex)
            throw new InvalidStackValueTypeException(StackValueKind.ReturnIndex, returnIndexValue.Kind);

        var returnIndex = (long)returnIndexValue.Value;
        instance.ProcedureIndex = (int)(returnIndex >> 32);
        instance.InstructionIndex = (int)returnIndex + 1;
        return true;
    }

    private static bool JUMP(FlowScriptInterpreter instance)
    {
        instance.ProcedureIndex = instance.Instruction.Operand.UInt16Value;
        instance.InstructionIndex = 0;
        return true;
    }

    private static bool CALL(FlowScriptInterpreter instance)
    {
        instance.PushValue(StackValueKind.ReturnIndex, ((long)instance.ProcedureIndex << 32) | (long)instance.InstructionIndex);
        instance.ProcedureIndex = instance.Instruction.Operand.UInt16Value;
        instance.InstructionIndex = 0;
        return true;
    }

    private static bool RUN(FlowScriptInterpreter instance)
    {
        throw new NotImplementedException();
    }

    private static bool GOTO(FlowScriptInterpreter instance)
    {
        var index = instance.Instruction.Operand.UInt16Value;
        instance.InstructionIndex = instance.Procedure.Labels[index].InstructionIndex;
        return true;
    }

    private static bool ADD(FlowScriptInterpreter instance)
    {
        var l = instance.PopArithmicValue();
        var r = instance.PopArithmicValue();
        instance.PushValue((dynamic)l.Value + (dynamic)r.Value);
        return true;
    }

    private static bool SUB(FlowScriptInterpreter instance)
    {
        var l = instance.PopArithmicValue();
        var r = instance.PopArithmicValue();
        instance.PushValue((dynamic)l.Value - (dynamic)r.Value);
        return true;
    }

    private static bool MUL(FlowScriptInterpreter instance)
    {
        var l = instance.PopArithmicValue();
        var r = instance.PopArithmicValue();
        instance.PushValue((dynamic)l.Value * (dynamic)r.Value);
        return true;
    }

    private static bool DIV(FlowScriptInterpreter instance)
    {
        var l = instance.PopArithmicValue();
        var r = instance.PopArithmicValue();
        instance.PushValue((dynamic)l.Value / (dynamic)r.Value);
        return true;
    }

    private static bool MINUS(FlowScriptInterpreter instance)
    {
        var l = instance.PopArithmicValue();
        instance.PushValue(-(dynamic)l.Value);
        return true;
    }

    private static bool NOT(FlowScriptInterpreter instance)
    {
        var o = instance.PopBooleanValue();
        instance.PushValue(!o);
        return true;
    }

    private static bool OR(FlowScriptInterpreter instance)
    {
        var l = instance.PopBooleanValue();
        var r = instance.PopBooleanValue();
        instance.PushValue(l || r);
        return true;
    }

    private static bool AND(FlowScriptInterpreter instance)
    {
        var l = instance.PopBooleanValue();
        var r = instance.PopBooleanValue();
        instance.PushValue(l && r);
        return true;
    }

    private static bool EQ(FlowScriptInterpreter instance)
    {
        var l = instance.PopArithmicValue();
        var r = instance.PopArithmicValue();
        instance.PushValue((dynamic)l.Value == (dynamic)r.Value);
        return true;
    }

    private static bool NEQ(FlowScriptInterpreter instance)
    {
        var l = instance.PopArithmicValue();
        var r = instance.PopArithmicValue();
        instance.PushValue((dynamic)l.Value != (dynamic)r.Value);
        return true;
    }

    private static bool S(FlowScriptInterpreter instance)
    {
        var l = instance.PopArithmicValue();
        var r = instance.PopArithmicValue();
        instance.PushValue((dynamic)l.Value < (dynamic)r.Value);
        return true;
    }

    private static bool L(FlowScriptInterpreter instance)
    {
        var l = instance.PopArithmicValue();
        var r = instance.PopArithmicValue();
        instance.PushValue((dynamic)l.Value > (dynamic)r.Value);
        return true;
    }

    private static bool SE(FlowScriptInterpreter instance)
    {
        var l = instance.PopArithmicValue();
        var r = instance.PopArithmicValue();
        instance.PushValue((dynamic)l.Value <= (dynamic)r.Value);
        return true;
    }

    private static bool LE(FlowScriptInterpreter instance)
    {
        var l = instance.PopArithmicValue();
        var r = instance.PopArithmicValue();
        instance.PushValue((dynamic)l.Value >= (dynamic)r.Value);
        return true;
    }

    private static bool IF(FlowScriptInterpreter instance)
    {
        var condition = instance.PopBooleanValue();
        if (condition)
            return true;

        // Jump to false label
        var index = instance.Instruction.Operand.UInt16Value;
        var label = instance.Procedure.Labels[index];
        instance.InstructionIndex = label.InstructionIndex;
        return true;
    }

    private static bool PUSHIS(FlowScriptInterpreter instance)
    {
        var value = instance.Instruction.Operand.UInt16Value;
        instance.PushValue(value);
        return true;
    }

    private static bool PUSHLIX(FlowScriptInterpreter instance)
    {
        var index = instance.Instruction.Operand.UInt16Value;
        var value = instance.LocalIntVariables[index];
        instance.PushValue(value);
        return true;
    }

    private static bool PUSHLFX(FlowScriptInterpreter instance)
    {
        var index = instance.Instruction.Operand.UInt16Value;
        var value = instance.LocalFloatVariables[index];
        instance.PushValue(value);
        return true;
    }

    private static bool POPLIX(FlowScriptInterpreter instance)
    {
        var index = instance.Instruction.Operand.UInt16Value;
        instance.LocalIntVariables[index] = instance.PopIntValue();
        return true;
    }

    private static bool POPLFX(FlowScriptInterpreter instance)
    {
        var index = instance.Instruction.Operand.UInt16Value;
        instance.LocalFloatVariables[index] = instance.PopFloatValue();
        return true;
    }

    private static bool PUSHSTR(FlowScriptInterpreter instance)
    {
        instance.PushValue(instance.Instruction.Operand.StringValue);
        return true;
    }

    private static bool POPREG(FlowScriptInterpreter instance)
    {
        instance.mREGValue = instance.PopValue();
        return true;
    }
}
