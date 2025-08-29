using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CaseMorphExtension.Core;

[AttributeUsage(AttributeTargets.Method)]
public class TransformationDisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    public string Glyph { get; }

    public TransformationDisplayNameAttribute(string displayName, string glyph = "\uE8E9")
    {
        DisplayName = displayName;
        Glyph = glyph;
    }
}

public static class TextTransformer
{
    private static string[] SplitWords(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return [string.Empty];

        return input.Split(new char[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
    }

    [TransformationDisplayName("Sentence case", "\uE8E9")]
    public static string ToSentenceCase(string text)
    {
        var sb = new StringBuilder(text.Length);
        bool capitalizeNext = true;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (char.IsLetter(c) && capitalizeNext)
            {
                sb.Append(char.ToUpperInvariant(c));
                capitalizeNext = false;
            }
            else
            {
                sb.Append(c);
            }

            if (c == '.' || c == '!' || c == '?')
            {
                capitalizeNext = true;
            }
        }
        return sb.ToString();
    }

    [TransformationDisplayName("lowercase", "\uE84A")]
    public static string ToLowercase(string input)
    {
        return input.ToLowerInvariant();
    }

    [TransformationDisplayName("UPPERCASE", "\uE84B")]
    public static string ToUppercase(string input)
    {
        return input.ToUpperInvariant();
    }

    [TransformationDisplayName("iNVERSE cASE", "\uE8E7")]
    public static string ToInverseCase(string input)
    {
        char[] chars = input.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (char.IsUpper(chars[i]))
            {
                chars[i] = char.ToLowerInvariant(chars[i]);
            }
            else if (char.IsLower(chars[i]))
            {
                chars[i] = char.ToUpperInvariant(chars[i]);
            }
        }
        return new string(chars);
    }

    [TransformationDisplayName("AlTeRnAte Case", "\uE8E8")]
    public static string ToAlternateCase(string input)
    {
        char[] chars = input.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (i % 2 == 0)
            {
                chars[i] = char.ToLowerInvariant(chars[i]);
            }
            else
            {
                chars[i] = char.ToUpperInvariant(chars[i]);
            }
        }
        return new string(chars);
    }

    [TransformationDisplayName("Title Case", "\uE8D2")]
    public static string ToTitleCase(string input)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        string result = textInfo.ToTitleCase(input.ToLowerInvariant());
        return result;
    }

    [TransformationDisplayName("camelCase", "\uE8AC")]
    public static string ToCamelCase(string input)
    {
        var words = SplitWords(input);

        StringBuilder sb = new StringBuilder(words[0].ToLowerInvariant());
        
        for (int i = 1; i < words.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(words[i]))
            {
                sb.Append(char.ToUpperInvariant(words[i][0]) + words[i].Substring(1).ToLowerInvariant());
            }
        }

        return sb.ToString();
    }

    [TransformationDisplayName("PascalCase", "\uF093")]
    public static string ToPascalCase(string input)
    {
        var words = SplitWords(input);
        StringBuilder sb = new StringBuilder();
        
        foreach (var word in words)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                sb.Append(char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant());
            }
        }
        return sb.ToString();
    }

    [TransformationDisplayName("snake_case", "\uE8DC")] 
    public static string ToSnakeCase(string input)
    {
        return string.Join("_", SplitWords(input).Select(w => w.ToLowerInvariant()));
    }

    [TransformationDisplayName("CONSTANT_CASE", "\uE97F")]
    public static string ToConstantCase(string input)
    {
        return string.Join("_", SplitWords(input).Select(w => w.ToUpperInvariant()));
    }

    [TransformationDisplayName("kebab-case", "\uE94D")]
    public static string ToKebabCase(string input)
    {
        return string.Join("-", SplitWords(input).Select(w => w.ToLowerInvariant()));
    }

    [TransformationDisplayName("COBOL-CASE", "\uE8D3")]
    public static string ToCobolCase(string input)
    {
        return string.Join("-", SplitWords(input).Select(w => w.ToUpperInvariant()));
    }

    [TransformationDisplayName("Train-Case", "\uEDE0")]
    public static string ToTrainCase(string input)
    {
        var words = SplitWords(input);
        StringBuilder sb = new StringBuilder();

        foreach (var word in words)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                if (sb.Length > 0) sb.Append('-');
                sb.Append(char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant());
            }
        }
        return sb.ToString();
    }

    [TransformationDisplayName("dot.case", "\uE843")]
    public static string ToDotCase(string input)
    {
        return string.Join(".", SplitWords(input).Select(w => w.ToLowerInvariant()));
    }

    [TransformationDisplayName("path/case", "\uE8B7")]
    public static string ToPathCase(string input)
    {
        return string.Join("/", SplitWords(input).Select(w => w.ToLowerInvariant()));
    }

    [TransformationDisplayName(@"path\case\backslash", "\uED41")]
    public static string ToPathBackslashCase(string input)
    {
        return string.Join("\\", SplitWords(input).Select(w => w.ToLowerInvariant()));
    }

    [TransformationDisplayName("Rèmòvè àccènts", "\uF2B7")]
    public static string RemoveDiacritics(string input)
    {
        var normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    [TransformationDisplayName("Remove? special! characters&", "\uED60")]
    public static string RemoveSpecialCharacters(string input)
    {
        return new string(input.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
    }

    [TransformationDisplayName("Remove multiple   spaces \nnewlines and\ttabs", "\uEF17")]
    public static string RemoveDuplicateWhitespace(string input)
    {
        return string.Join(" ", input.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
    }
}