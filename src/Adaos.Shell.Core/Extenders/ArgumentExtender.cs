using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Core.Extenders
{
    public static class ArgumentExtender
    {
        public static bool VerifyArgumentCount(this IEnumerable<IArgument> args, int count, Action<string> errorHandler = null)
        {
            return args.VerifyArgumentCount(count, count, errorHandler);
        }

        public static bool VerifyArgumentCount(this IEnumerable<IArgument> args, int min, int max, Action<string> errorHandler = null)
        {
            return args.VerifyArgumentMaxCount(max, errorHandler) && args.VerifyArgumentMinCount(min, errorHandler);
        }

        public static bool VerifyArgumentMaxCount(this IEnumerable<IArgument> args, int max, Action<string> errorHandler = null)
        {
            if (args.Skip(max).FirstOrDefault() != null)
            {
                errorHandler?.Invoke("Too many arguments. " + args.Count() + " received, at most" + max + " is allowed.");
                return false;
            }
            return true;
        }

        public static bool VerifyArgumentMinCount(this IEnumerable<IArgument> args, int min, Action<string> errorHandler = null)
        {
            if (min == 0) return true;
            if (args.Skip(min-1).FirstOrDefault() == null /*count < min*/)
            {
                errorHandler?.Invoke("Too few arguments. " + args.Count() + " received, at least " + min + " is required.");
                return false;
            }
            return true;
        }

        public static bool TryParseTo(this IArgument arg, out double result, Action<string> errorHandler = null)
        {
            if (double.TryParse(arg.Value, out result))
            {
                return true;
            }
            else
            {
                errorHandler?.Invoke("Unable to parse: '" + arg.Value + "' to a double");
                return false;
            }
        }

        public static bool TryParseTo(this IArgument arg, out bool result, Action<string> errorHandler = null)
        {
            if (bool.TryParse(arg.Value, out result))
            {
                return true;
            }
            else
            {
                errorHandler?.Invoke("Unable to parse: '" + arg.Value + "' to a bool");
                return false;
            }
        }

        public static bool TryParseTo(this IArgument arg, out int result, Action<string> errorHandler = null)
        {
            if (int.TryParse(arg.Value, out result))
            {
                return true;
            }
            else
            {
                errorHandler?.Invoke("Unable to parse: '" + arg.Value + "' to a int");
                return false;
            }
        }

        public static int ParseInt(this IArgument arg, Action<string> errorHandler = null)
        {
            int res = 0;
            arg.TryParseTo(out res, errorHandler);
            return res;
        }

        public static double ParseDouble(this IArgument arg, Action<string> errorHandler = null)
        {
            double res = 0;
            arg.TryParseTo(out res, errorHandler);
            return res;
        }

        public static IEnumerable<KeyValuePair<string, IArgument>> ParseOptionsAndFlagsWithAlias(
            this IEnumerable<IArgument> self, 
            List<string[]> optionNamesWithAlias, 
            List<string[]> flagNamesWithAlias)
        {
            IArgument[] arr = self.ToArray();
            string[] toRemove;
            for (int i = 0; i < arr.Length; i++)
            {
                toRemove = null;
                foreach (var flag in flagNamesWithAlias)
                {
                    foreach (var alias in flag)
                    {
                        if (arr[i].Value.ToLower().Equals(alias.ToLower()))
                        {
                            if (i > arr.Length)
                            {
                                yield break;
                            }
                            yield return new KeyValuePair<string, IArgument>(flag[0].ToLower(), arr[i]);
                            toRemove = flag;
                            break;
                        }
                    }
                }
                if (toRemove != null)
                {
                    flagNamesWithAlias.Remove(toRemove);
                }
                else
                {
                    foreach (var opt in optionNamesWithAlias)
                    {
                        foreach (var alias in opt)
                        {
                            if (arr[i].Value.ToLower().Equals(alias.ToLower()))
                            {
                                i++;
                                if (i > arr.Length)
                                {
                                    yield break;
                                }
                                yield return new KeyValuePair<string, IArgument>(opt[0].ToLower(), arr[i]);
                                toRemove = opt;
                                break;
                            }
                        }
                    }
                }
                if (toRemove != null)
                {
                    optionNamesWithAlias.Remove(toRemove);
                }
                else
                {
                    throw new SemanticException(-1, arr[i].Value + " is not a valid option");
                }
            }
        }

        public static IEnumerable<KeyValuePair<string,IArgument>> ParseOptions(this IEnumerable<IArgument> self, List<string> optionNames)
        {
            List<string[]> withAlias = optionNames.Select(x => new string[1]{x}).ToList();
            return ParseOptionsWithAlias(self, withAlias);
        }

        /// <summary>
        /// Parses a Enumerable of argument to a set of key pair values with the options specified in 
        /// <paramref name="optionNamesWithAlias"/> and the option retried from the enumerable calling this.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="optionNamesWithAlias">A list of options with aliases to be retrieved values for.
        /// The first entry in each array in the list is the actual key in the result, the rest are aliases</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, IArgument>> ParseOptionsWithAlias(this IEnumerable<IArgument> self, List<string[]> optionNamesWithAlias, bool strict = true)
        {
            IArgument[] arr = self.ToArray();
            string[] toRemove;
            for (int i = 0; i < arr.Length; i++)
            {
                toRemove = null;
                foreach (var opt in optionNamesWithAlias)
                {
                    foreach (var alias in opt)
                    {
                        if (arr[i].Value.ToLower().Equals(alias.ToLower()))
                        {
                            i++;
                            if (i > arr.Length)
                            {
                                yield break;
                            }
                            yield return new KeyValuePair<string, IArgument>(opt[0].ToLower(), arr[i]);
                            toRemove = opt;
                            break;
                        }
                    }
                }
                if (toRemove != null)
                {
                    optionNamesWithAlias.Remove(toRemove);
                }
                else if(strict)
                {
                    throw new SemanticException(-1, arr[i].Value + " is not a valid option");
                }
            }
        }

        public static IEnumerable<KeyValuePair<string, bool>> ParseFlagsWithAlias(this IEnumerable<IArgument> self, List<string[]> flagNamesWithAlias,bool strict = true)
        {
            IArgument[] arr = self.ToArray();
            string[] toRemove;
            for (int i = 0; i < arr.Length; i++)
            {
                toRemove = null;
                foreach (var opt in flagNamesWithAlias)
                {
                    foreach (var alias in opt)
                    {
                        if (arr[i].Value.ToLower().Equals(alias.ToLower()))
                        {
                            if (i > arr.Length)
                            {
                                yield break;
                            }
                            yield return new KeyValuePair<string, bool>(opt[0].ToLower(), true);
                            toRemove = opt;
                            break;
                        }
                    }
                }
                if (toRemove != null)
                {
                    flagNamesWithAlias.Remove(toRemove);
                }
                else if (strict)
                {
                    throw new SemanticException(-1, arr[i].Value + " is not a valid flag");
                }
            }

            foreach (var rest in flagNamesWithAlias)
            {
                yield return new KeyValuePair<string, bool>(rest[0].ToLower(), false);
            }
        }

        public static IEnumerable<KeyValuePair<string, bool>> ParseFlags(this IEnumerable<IArgument> self, List<string> flagNames)
        { 
            List<string[]> flagNamesWithAlias = flagNames.Select(x => new string[1]{x}).ToList();
            return ParseFlagsWithAlias(self, flagNamesWithAlias);
        }
    }
}
