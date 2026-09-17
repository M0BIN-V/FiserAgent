using System.Text;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Interfaces.TelegramBot;

public sealed class TelegramMarkdownV2Renderer
{
    private static readonly MarkdownPipeline Pipeline =
        new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

    /// <summary>
    ///     Converts standard Markdown into Telegram MarkdownV2.
    /// </summary>
    public string Render(string markdown)
    {
        ArgumentNullException.ThrowIfNull(markdown);

        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var document = Markdown.Parse(markdown, Pipeline);

        var output = new StringBuilder();

        foreach (var block in document)
            RenderBlock(block, output);

        return Normalize(output.ToString());
    }

    /// <summary>
    ///     Escapes plain text for Telegram MarkdownV2.
    /// </summary>
    public static string EscapeText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var output = new StringBuilder(text.Length);

        WriteEscapedText(text, output);

        return output.ToString();
    }

    /// <summary>
    ///     Renders a value as Telegram MarkdownV2 inline code.
    /// </summary>
    public static string RenderInlineCode(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var output = new StringBuilder();

        RenderInlineCode(text, output);

        return output.ToString();
    }

    private static void RenderBlock(
        Block block,
        StringBuilder output)
    {
        switch (block)
        {
            case HeadingBlock heading:
                RenderHeading(heading, output);
                break;

            case ParagraphBlock paragraph:
                RenderInlineContainer(
                    paragraph.Inline,
                    output);

                output.Append("\n\n");
                break;

            case FencedCodeBlock fencedCode:
                RenderCodeBlock(fencedCode, output);
                output.Append("\n\n");
                break;

            case CodeBlock codeBlock:
                RenderCodeBlock(codeBlock, output);
                output.Append("\n\n");
                break;

            case QuoteBlock quote:
                RenderQuote(quote, output);
                output.Append("\n\n");
                break;

            case ListBlock list:
                RenderList(list, output);
                output.Append('\n');
                break;

            case ThematicBreakBlock:
                output.Append("\\-\\-\\-");
                output.Append("\n\n");
                break;

            case ContainerBlock container:
                foreach (var child in container)
                    if (child is Block childBlock)
                        RenderBlock(childBlock, output);

                break;
        }
    }

    private static void RenderHeading(
        HeadingBlock heading,
        StringBuilder output)
    {
        output.Append('*');

        RenderInlineContainer(
            heading.Inline,
            output);

        output.Append('*');
        output.Append("\n\n");
    }

    private static void RenderInlineContainer(
        ContainerInline? container,
        StringBuilder output)
    {
        if (container is null)
            return;

        foreach (var inline in container)
            RenderInline(inline, output);
    }

    private static void RenderInline(
        Inline inline,
        StringBuilder output)
    {
        switch (inline)
        {
            case LiteralInline literal:
                WriteEscapedText(
                    literal.Content.ToString(),
                    output);
                break;

            case CodeInline code:
                RenderInlineCode(
                    code.Content,
                    output);
                break;

            case LineBreakInline:
                output.Append('\n');
                break;

            case LinkInline link when !link.IsImage:
                RenderLink(link, output);
                break;

            case LinkInline link:
                // Telegram MarkdownV2 doesn't have a normal
                // Markdown image syntax for regular messages.
                // Render image alt text instead.
                RenderImage(link, output);
                break;

            case EmphasisInline emphasis:
                RenderEmphasis(
                    emphasis,
                    output);
                break;

            case AutolinkInline autolink:
                RenderAutolink(
                    autolink,
                    output);
                break;

            case HtmlInline html:
                // Telegram MarkdownV2 doesn't support arbitrary HTML.
                // Treat HTML as normal text.
                WriteEscapedText(
                    html.Tag,
                    output);
                break;

            case HtmlEntityInline entity:
                WriteEscapedText(
                    entity.Transcoded.ToString(),
                    output);
                break;

            case ContainerInline container:
                RenderInlineContainer(
                    container,
                    output);
                break;
        }
    }

    private static void RenderInlineCode(
        string value,
        StringBuilder output)
    {
        output.Append('`');

        foreach (var character in value)
        {
            if (character is '`' or '\\')
                output.Append('\\');

            output.Append(character);
        }

        output.Append('`');
    }

    private static void RenderEmphasis(
        EmphasisInline emphasis,
        StringBuilder output)
    {
        var marker = GetEmphasisMarker(emphasis);

        output.Append(marker);

        RenderInlineContainer(
            emphasis,
            output);

        output.Append(marker);
    }

    private static string GetEmphasisMarker(
        EmphasisInline emphasis)
    {
        return emphasis.DelimiterChar switch
        {
            // Telegram:
            // ~text~ = strikethrough
            '~' => "~",

            // Telegram:
            // __text__ = underline
            '_' when emphasis.DelimiterCount >= 2
                => "__",

            // Telegram:
            // *text* = bold
            '*' when emphasis.DelimiterCount >= 2
                => "*",

            // Telegram:
            // _text_ = italic
            '_' => "_",

            '*' => "_",

            _ => "_"
        };
    }

    private static void RenderLink(
        LinkInline link,
        StringBuilder output)
    {
        output.Append('[');

        RenderInlineContainer(
            link,
            output);

        output.Append("](");

        WriteEscapedUrl(
            link.Url ?? string.Empty,
            output);

        output.Append(')');
    }

    private static void RenderImage(
        LinkInline image,
        StringBuilder output)
    {
        // Markdown images are not directly supported by
        // regular Telegram MarkdownV2 messages.
        //
        // Keep the alt text rather than leaking Markdown syntax.

        output.Append('[');

        RenderInlineContainer(
            image,
            output);

        output.Append(']');
    }

    private static void RenderAutolink(
        AutolinkInline autolink,
        StringBuilder output)
    {
        var url = autolink.Url ?? string.Empty;

        output.Append('[');

        WriteEscapedText(
            url,
            output);

        output.Append("](");

        WriteEscapedUrl(
            url,
            output);

        output.Append(')');
    }

    private static void RenderCodeBlock(
        LeafBlock block,
        StringBuilder output)
    {
        output.Append("```");

        if (block is FencedCodeBlock fenced &&
            !string.IsNullOrWhiteSpace(fenced.Info))
            output.Append(
                SanitizeLanguage(fenced.Info));

        output.Append('\n');

        foreach (var line in block.Lines.Lines)
        {
            var value = line.Slice.ToString();

            WriteEscapedCode(
                value,
                output);

            output.Append('\n');
        }

        output.Append("```");
    }

    private static void RenderQuote(
        QuoteBlock quote,
        StringBuilder output)
    {
        var content = new StringBuilder();

        foreach (var block in quote)
            if (block is Block child)
                RenderBlock(
                    child,
                    content);

        var lines = content
            .ToString()
            .TrimEnd('\n')
            .Split('\n');

        foreach (var line in lines)
        {
            output.Append('>');

            // The content has already been rendered into
            // Telegram MarkdownV2.
            output.Append(line);

            output.Append('\n');
        }
    }

    private static void RenderList(
        ListBlock list,
        StringBuilder output)
    {
        var index = 1;

        foreach (var block in list)
        {
            if (block is not ListItemBlock item)
                continue;

            var itemContent = new StringBuilder();

            foreach (var child in item)
                if (child is Block childBlock)
                    RenderBlock(
                        childBlock,
                        itemContent);

            var content = itemContent
                .ToString()
                .TrimEnd('\n');

            if (string.IsNullOrWhiteSpace(content))
                continue;

            var lines = content.Split('\n');

            var prefix = list.IsOrdered
                ? $"{index}\\. "
                : "\\- ";

            output.Append(prefix);
            output.Append(lines[0]);

            for (var i = 1; i < lines.Length; i++)
            {
                output.Append('\n');
                output.Append("  ");
                output.Append(lines[i]);
            }

            output.Append('\n');

            index++;
        }
    }

    private static void WriteEscapedText(
        string value,
        StringBuilder output)
    {
        foreach (var character in value)
        {
            if (IsMarkdownV2SpecialCharacter(character))
                output.Append('\\');

            output.Append(character);
        }
    }

    private static void WriteEscapedCode(
        string value,
        StringBuilder output)
    {
        foreach (var character in value)
        {
            if (character is '`' or '\\')
                output.Append('\\');

            output.Append(character);
        }
    }

    private static void WriteEscapedUrl(
        string url,
        StringBuilder output)
    {
        foreach (var character in url)
        {
            if (character is ')' or '\\')
                output.Append('\\');

            output.Append(character);
        }
    }

    private static bool IsMarkdownV2SpecialCharacter(
        char character)
    {
        return character is
            '\\'
            or '_'
            or '*'
            or '['
            or ']'
            or '('
            or ')'
            or '~'
            or '`'
            or '>'
            or '#'
            or '+'
            or '-'
            or '='
            or '|'
            or '{'
            or '}'
            or '.'
            or '!';
    }

    private static string SanitizeLanguage(
        string language)
    {
        var result = new StringBuilder();

        foreach (var character in language)
            if (char.IsLetterOrDigit(character))
                result.Append(character);

        return result.ToString();
    }

    private static string Normalize(
        string value)
    {
        while (value.Contains("\n\n\n"))
            value = value.Replace(
                "\n\n\n",
                "\n\n");

        return value.Trim();
    }
}