
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OrisProject.Data;

public class TemplateEngine
{
    // Подстановка переменных и обработка простых условий
    public static string Render(
        string template,
        Dictionary<string, string> data)
    {
        // Обработка условий {{#if Name}} ... {{/if}}
        string ifPattern =
            @"\{\{#if\s+(\w+)\s*\}\}(.*?)\{\{/if\}\}";

        template = Regex.Replace(
            template,
            ifPattern,
            match =>
            {
                string key = match.Groups[1].Value;
                string content = match.Groups[2].Value;

                if (data.TryGetValue(key, out string? value)
                    && !string.IsNullOrWhiteSpace(value)
                    && value != "false"
                    && value != "0")
                {
                    return content;
                }

                return "";
            },
            RegexOptions.Singleline
        );

        // Подстановка обычных переменных {{Name}}
        foreach (var item in data)
        {
            string pattern =
                $@"\{{\{{\s*{Regex.Escape(item.Key)}\s*\}}\}}";

            template = Regex.Replace(
                template,
                pattern,
                _ => item.Value ?? ""
            );
        }

        return template;
    }

    // Обработка циклов
    public static string RenderList<T>(
        string template,
        string tagName,
        List<T> items,
        Func<T, string> templateBlock)
    {
        string pattern =
            $@"\{{\{{#each\s+{Regex.Escape(tagName)}\s*\}}\}}(.*?)\{{\{{\/each\}}\}}";

        return Regex.Replace(
            template,
            pattern,
            match =>
            {
                string result = "";

                foreach (var item in items)
                {
                    // Генерируем HTML для каждой карточки
                    result += templateBlock(item);
                }

                return result;
            },
            RegexOptions.Singleline
        );
    }
}
