using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RuleEngileDecorator
{
    public interface IRuleAttribut
    {
        string Category { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RuleAttribute : Attribute, IRuleAttribut
    {
        public int priority;
        public string Category { get; set; }

        public RuleAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RunAttribute : Attribute
    {
        public string Category;

        public RunAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RunAsyncAttribute : Attribute
    {
        public string Category;

        public RunAsyncAttribute()
        {
        }
    }

    public enum Priority
    {
        Ascending,
        Descending,
    }

    public class RuleEngine
    {
        public Priority Priority { get; set; }
        public string Category { get; set; }
        public object RuleContext { get; set; }

        public RuleEngine(string c, object ruleContext)
        {
            Category = c;

            RuleContext = ruleContext;   
        }

        public bool IsValid()
        {
            var rules = Util
                .GetTypesDecoratedWith<RuleAttribute>(Category);

            var result = rules.All(rule =>
            {
                var ruleInstance = Activator.CreateInstance(rule);

                var validationMethods = rule.GetMethods().Where(m =>
                    m.GetCustomAttribute<RunAttribute>() != null);

                var isValid = validationMethods?.All(r => {
                    var result = r.Invoke(ruleInstance, new[] { RuleContext });

                    return result is bool isValid && isValid;
                }) ?? true;

                return isValid;
            });

            return result;
        }

        public async Task<bool> IsValidAsync()
        {
            var rules = Util
                .GetTypesDecoratedWith<RuleAttribute>(Category);

            var areAllRulesValid = true;

            foreach (var rule in rules)
            {
                var ruleInstance = Activator.CreateInstance(rule);

                var asyncValidationMethods = rule.GetMethods().Where(m =>
                    m.GetCustomAttribute<RunAsyncAttribute>() != null);

                if (asyncValidationMethods == null)
                {
                    continue;
                }

                var isValid = true;

                foreach (var m in asyncValidationMethods)
                {
                    var task = (Task<bool>)m.Invoke(ruleInstance, new[] { RuleContext });

                    var result = await task;

                    if (result is bool isOk && isOk == false)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid == false)
                {
                    areAllRulesValid = false;
                    break;
                }
            }

            return areAllRulesValid;
        }
    }

    public class Util
    {
        public static IEnumerable<Type> GetTypesDecoratedWith<T>(string of)
            where T : Attribute, IRuleAttribut

        // Gets the assembly that contains this RuleEngine code.
        // Meaning it will return the assembly containing the method
        // that is calling Assembly.GetExecutingAssembly().
        => Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                Attribute.IsDefined(t, typeof(T)) &&
                t.GetCustomAttributes().Any(a =>
       
                    a is T bl && bl.Category == of
                )
            );
    }
}
