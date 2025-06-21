using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CaseMorphExtension.Core;

[AttributeUsage(AttributeTargets.Method)]
public class TransformationDisplayNameAttribute : Attribute
{
    public string DisplayName { get; }

    public TransformationDisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}

public static class TextTransformer
{
    [TransformationDisplayName("Sentence case")]
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

    [TransformationDisplayName("lowercase")]
    public static string ToLowercase(string input)
    {
        return input.ToLowerInvariant();
    }

    [TransformationDisplayName("UPPERCASE")]
    public static string ToUppercase(string input)
    {
        return input.ToUpperInvariant();
    }

    [TransformationDisplayName("AlTeRnAte Case")]
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

    [TransformationDisplayName("iNVERSE cASE")]
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

    private static string[] SplitWords(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Array.Empty<string>();

        return input.Split(new char[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
    }

    [TransformationDisplayName("Title Case")]
    public static string ToTitleCase(string input)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        string result = textInfo.ToTitleCase(input.ToLowerInvariant());

        return result;
    }

    [TransformationDisplayName("camelCase")]
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

    [TransformationDisplayName("PascalCase")]
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

    [TransformationDisplayName("snake_case")]
    public static string ToSnakeCase(string input)
    {
        return string.Join("_", SplitWords(input).Select(w => w.ToLowerInvariant()));
    }

    [TransformationDisplayName("CONSTANT_CASE")]
    public static string ToConstantCase(string input)
    {
        return string.Join("_", SplitWords(input).Select(w => w.ToUpperInvariant()));
    }

    [TransformationDisplayName("kebab-case")]
    public static string ToKebabCase(string input)
    {
        return string.Join("-", SplitWords(input).Select(w => w.ToLowerInvariant()));
    }

    [TransformationDisplayName("COBOL-CASE")]
    public static string ToCobolCase(string input)
    {
        return string.Join("-", SplitWords(input).Select(w => w.ToUpperInvariant()));
    }

    [TransformationDisplayName("Train-Case")]
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
}