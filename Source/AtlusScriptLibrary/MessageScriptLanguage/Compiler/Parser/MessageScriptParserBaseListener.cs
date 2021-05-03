//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.6.4
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from ..\..\..\AtlusScriptLibrary\MessageScriptLanguage\Compiler\Parser\MessageScriptParser.g4 by ANTLR 4.6.4

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace AtlusScriptLibrary.FlowScriptLanguage.Compiler.Parser {

using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IMessageScriptParserListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6.4")]
[System.CLSCompliant(false)]
public partial class MessageScriptParserBaseListener : IMessageScriptParserListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="MessageScriptParser.compilationUnit"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCompilationUnit([NotNull] MessageScriptParser.CompilationUnitContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MessageScriptParser.compilationUnit"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCompilationUnit([NotNull] MessageScriptParser.CompilationUnitContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="MessageScriptParser.dialog"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDialog([NotNull] MessageScriptParser.DialogContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MessageScriptParser.dialog"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDialog([NotNull] MessageScriptParser.DialogContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="MessageScriptParser.messageDialog"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMessageDialog([NotNull] MessageScriptParser.MessageDialogContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MessageScriptParser.messageDialog"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMessageDialog([NotNull] MessageScriptParser.MessageDialogContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="MessageScriptParser.speakerName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSpeakerName([NotNull] MessageScriptParser.SpeakerNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MessageScriptParser.speakerName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSpeakerName([NotNull] MessageScriptParser.SpeakerNameContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="MessageScriptParser.selectionDialog"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSelectionDialog([NotNull] MessageScriptParser.SelectionDialogContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MessageScriptParser.selectionDialog"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSelectionDialog([NotNull] MessageScriptParser.SelectionDialogContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="MessageScriptParser.tokenText"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTokenText([NotNull] MessageScriptParser.TokenTextContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MessageScriptParser.tokenText"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTokenText([NotNull] MessageScriptParser.TokenTextContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="MessageScriptParser.token"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterToken([NotNull] MessageScriptParser.TokenContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MessageScriptParser.token"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitToken([NotNull] MessageScriptParser.TokenContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="MessageScriptParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpression([NotNull] MessageScriptParser.ExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MessageScriptParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpression([NotNull] MessageScriptParser.ExpressionContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
} // namespace AtlusScriptLibrary.FlowScriptLanguage.Compiler.Parser
