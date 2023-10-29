// See https://aka.ms/new-console-template for more information
// dotnet publish -r win-x64 -c Release

using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using PluginAPI.Core.Attributes;

public static class Program
{
    public static void Main(string[] args)
    {
        CosturaUtility.Initialize();
        bool ReplaceEnvironmentalVariables = true;
        Dictionary<string, string> replacementTerms = new Dictionary<string, string>();
        string fileLoc = "";
        List<string> EnvironmentalVariables = new List<string>();

        if (args.Length == 0)
        {
            Console.Error.WriteLine("No file specified. Could not replace variables.");
            return;
        }
        
        List<string> nextArguments = new List<string>();
        for (int i = 0; i < args.Length; i++)
        {
            string argument = args[i];
            //Console.WriteLine($"Argument: {argument}");
            nextArguments.Clear();
            if (argument.StartsWith('-'))
            {
                switch (argument)
                {
                    case "-d" or "--datetime":
                        if (args.TryProcessNextArguments(i, 1, out nextArguments))
                        {
                            i += 1;
                            replacementTerms.Add(nextArguments[0], DateTime.UtcNow.ToString("s"));
                        }
                        continue;
                    case "-v" or "--var":
                        //Console.WriteLine($"var");
                        if (args.TryProcessNextArguments(i, 2, out nextArguments))
                        {
                            replacementTerms.Add(nextArguments[0], nextArguments[1]);
                            i += 2;
                            Console.WriteLine($"var: {nextArguments[0]}, {nextArguments[1]}");
                        }
                        continue;

                    case "-ne" or "--no-environmental":
                        //Console.WriteLine($"no env");
                        ReplaceEnvironmentalVariables = false;
                        continue;

                    case "-e" or "--environmental":
                        //Console.WriteLine($"env");
                        if (args.TryProcessNextArguments(i, 1, out nextArguments))
                        {
                            EnvironmentalVariables.Add(nextArguments[0]);
                            i += 1;
                            //Console.WriteLine($"env: {nextArguments[0]}");
                        }
                        continue;
                    
                    case "-cmd" or "--cmd-variable":
                        if (args.TryProcessNextArguments(i, 2, out nextArguments))
                        {
                            i += 2;
                            string procName = nextArguments[0].Split(' ')[0];
                            string procArgs = nextArguments[0].Replace($"{procName} ", "");
                            //procArgs = "";
                            //Console.WriteLine($"starting process {procName}");
                            Process proc = new Process() { StartInfo = new ProcessStartInfo(procName,procArgs)
                            {
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            }};
                            proc.Start();
                            string output = "";
                            while (!proc.StandardOutput.EndOfStream)
                            {
                                output += proc.StandardOutput.ReadLine();
                            }
                            proc.WaitForExit();
                            //Console.WriteLine($"{output}");
                            replacementTerms.Add($"{nextArguments[1]}", output);
                        }
                        continue;
                    case "-bd" or "--build-dependencies":
                        if (args.TryProcessNextArguments(i, 2, out nextArguments))
                        {
                            i += 2; 
                            string folderLoc = Path.GetFullPath(nextArguments[0]);
                            if (!Directory.Exists(folderLoc))
                            {
                                Console.Error.WriteLine($"Could not find folder \"{folderLoc}\".");
                                continue;
                            }

                            string output = "";
                            List<AssemblyInfo> buildDependencies = new List<AssemblyInfo>();
                            foreach (string file in Directory.GetFiles(folderLoc, "*.dll"))
                            {
                                try
                                {
                                    Assembly assembly = null!;
                                    assembly = Assembly.Load(File.ReadAllBytes(file));
                                    
                                    var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "";
                                    AssemblyInfo dp = new AssemblyInfo()
                                    {
                                        Hash = assembly.ManifestModule.ModuleVersionId.ToString(),
                                        Name = assembly.GetName().ToString(),
                                        Exiled = false,
                                        Version = version,
                                    };
                                    if (CreatePlugin(assembly) is { } plif)
                                    {
                                        dp.Plugins.Add(plif);
                                    }

                                    var plugins = GetPluginInfo(assembly);
                                    if (plugins.Count > 0)
                                    {
                                        dp.Plugins.AddRange(plugins);
                                    }
                                    buildDependencies.Add(dp);
                                    //Console.WriteLine($"Loaded Assembly {dp.Name}");
                                }
                                catch (Exception e)
                                {
                                    // Console.Error.WriteLine($"Could not load assembly for file \"{file}\". Exception: \"{e.Message}\".");
                                    try
                                    {
                                        var info = dnlib.DotNet.AssemblyDef.Load(file);
                                        AssemblyInfo assemblyInfo = new AssemblyInfo()
                                        {
                                            Name = info.Name,
                                            Version = info.Version.ToString(),
                                            Hash = info.ManifestModule.Mvid.HasValue ? info.ManifestModule.Mvid.Value.ToString() : "",
                                        };
                                        foreach (var x in info.CustomAttributes)
                                        {
                                            if (x is null)
                                                continue;
                                            
                                            if (x.TypeFullName.ToLower().Contains(nameof(PluginEntryPoint).ToLower()))
                                            {
                                                
                                                Console.WriteLine($"");
                                            }
                                        }
                                        buildDependencies.Add(assemblyInfo);
                                        // Console.Error.WriteLine($"Could not load full assembly for {assemblyInfo.Name}. Loading Mini-Info instead.");
                                    }
                                    catch { }
                                }
                            }

                            output = JsonConvert.SerializeObject(buildDependencies).Replace("\"","\\\"");
                            replacementTerms.Add($"{nextArguments[1]}", output);
                        }
                        continue;
                    default:
                        Console.Error.WriteLine($"Unknown variable \"{argument}\"");
                        continue;

                }
            }
            else if (File.Exists(args[i]))
            {
                fileLoc = args[i];
                continue;
            }
            else
            {
                Console.Error.WriteLine($"Unknown Argument \"{argument}\"");
                return;
            }
        }

        if (fileLoc == "")
        {
            Console.Error.WriteLine($"File location must be specified.");
            return;
        }
        Dictionary<string, string> varsToReplace = new Dictionary<string, string>();

        if (ReplaceEnvironmentalVariables)
        {
            foreach (DictionaryEntry envVar in Environment.GetEnvironmentVariables())
            {
                //Console.WriteLine($"{(string)envVar.Key}, {(string)envVar.Value!}");
                varsToReplace.Add((string)envVar.Key,(string)envVar.Value!);
            }
        }

        string text = File.ReadAllText(fileLoc);

        foreach (var var in replacementTerms)
        {
            if (text.Contains(var.Key))
                varsToReplace.Add(var.Key, var.Value);
        }

        foreach (var x in varsToReplace)
        {
            text = text.Replace(x.Key, x.Value);
        }

        //Console.WriteLine($"Replacing {varsToReplace.Count + replacementTerms.Count} variables");
        File.WriteAllText(fileLoc, text);
        //Console.Write(text);
        Console.WriteLine("Replaced Text.");
    }

    private static bool TryProcessNextArguments(this string[] arguments, int currentTerm, int amountToSearch, out List<string> nextArguments)
    {
        nextArguments = new List<string>();
        try
        {
            if (arguments.Length < currentTerm + amountToSearch)
            {
                Console.Error.WriteLine($"Incorrect argument {arguments[currentTerm]}. Proper Usage: \n{ProperUsage(arguments[currentTerm])}");
                return false;
            }
            for (int i = currentTerm + 1; i <= currentTerm + amountToSearch; i++)
            {
                //Console.WriteLine($"{arguments[i]}");
                nextArguments.Add(arguments[i]);
            }
            return true;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Could not process next arguments for var {arguments[currentTerm]}. Error: {e}");
            return false;
        }
    }

    private static string ProperUsage(string var)
    {
        switch (var)
        {
            case "-v" or "--var":
                return "(--var / -v) \"string to replace\" \"replacement value\" - replaces a string with a value";
            
            case "-ne" or "--no-environmental":
                return "(--no-environemental / -ne) - prevents environmental variables from being swapped. (Use -e to add specific variables)";
            
            case "-e" or "--environmental":
                return "(--environmental / -e) \"environmental variable name\" - replaces an environmental variable.";
            case "-cmd" or "--cmd-variable":
                return "(--cmd-variable / -cmd) \"command to execute\" \"variable name\" - replaces an variable with the output of a command.";
            
        }

        return $"Could not find variable {var}";
    }

    private static List<PluginInfo> GetPluginInfo(Assembly assembly)
    {

        List<PluginInfo> plugins = new List<PluginInfo>();
        try
        {
            if (!Attribute.IsDefined(assembly, typeof(PluginEntryPoint)))
            {
                return plugins;
            }
            foreach (var type in assembly.GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    var entrypoint = method.GetCustomAttribute<PluginEntryPoint>();
                    if (entrypoint is null)
                    {
                        continue;
                    }
                    plugins.Add(new PluginInfo()
                    {
                        Authors = entrypoint.Author,
                        Name = entrypoint.Name,
                        ExiledPlugin = false,
                        Version = entrypoint.Version,
                        Descriptions = entrypoint.Description,
                    });
                }
            }
        }
        catch (Exception e)
        { }

        return plugins;
    }
        /// <summary>
        /// Create a plugin instance.
        /// </summary>
        /// <param name="assembly">The plugin assembly.</param>
        /// <returns>Returns the created plugin instance or <see langword="null"/>.</returns>
        private static PluginInfo? CreatePlugin(Assembly assembly)
        {
            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface)
                    {
                        continue;
                    }

                    if (!IsDerivedFromPlugin(type))
                    {
                        continue;
                    }


                    IPlugin<IConfig> plugin = null;

                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor is not null)
                    {
                     
                        plugin = constructor.Invoke(null) as IPlugin<IConfig>;
                    }
                    else
                    {

                        object value = Array.Find(type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public), property => property.PropertyType == type)?.GetValue(null);

                        if (value is not null)
                            plugin = value as IPlugin<IConfig>;
                    }

                    if (plugin is null)
                    {

                        continue;
                    }



                    return new PluginInfo()
                    {
                        ExiledPlugin = true,
                        Authors = plugin.Author,
                        Name = plugin.Name,
                        Version = plugin.Version.ToString(),
                    };
                }
            }
            catch (ReflectionTypeLoadException reflectionTypeLoadException)
            {
                //Log.Error($"Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {reflectionTypeLoadException}");

                foreach (Exception loaderException in reflectionTypeLoadException.LoaderExceptions)
                {
                    //Log.Error(loaderException);
                }
            }
            catch (Exception exception)
            {
                //Log.Error($"Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {exception}");
            }

            return null;
        }
        
        /// <summary>
        /// Indicates that the passed type is derived from the plugin type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns><see langword="true"/> if passed type is derived from <see cref="Plugin{TConfig}"/> or <see cref="Plugin{TConfig, TTranslation}"/>, otherwise <see langword="false"/>.</returns>
        private static bool IsDerivedFromPlugin(Type type)
        {
            while (type is not null)
            {
                type = type.BaseType;

                if (type is { IsGenericType: true })
                {
                    Type genericTypeDef = type.GetGenericTypeDefinition();

                    if (genericTypeDef == typeof(Plugin<>) || genericTypeDef == typeof(Plugin<,>))
                        return true;
                }
            }

            return false;
        }

    internal struct AssemblyInfo
    {
        public AssemblyInfo()
        {
            Plugins = new List<PluginInfo>();
            Exiled = false;
            Dependency = false;
            Name = "";
            Hash = "";
            Version = "";
        }
        public bool Exiled { get; set; }
        public bool Dependency { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string Version { get; set; }
        public List<PluginInfo> Plugins { get; set; }
    }

    internal struct PluginInfo
    {
        public PluginInfo()
        {
            ExiledPlugin = false;
            Name = "";
            Version = "";
            Authors = "";
            Descriptions = "";
        }
        public bool ExiledPlugin { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Authors { get; set; }
        public string Descriptions { get; set; }
    }
}