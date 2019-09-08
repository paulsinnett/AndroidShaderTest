using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class Build : IPreprocessShaders
{
    public int callbackOrder { get { return 0; } }

    static Dictionary<string, int> variants =
        new Dictionary<string, int>();

    static Dictionary<string, int> shaderKeywords =
        new Dictionary<string, int>();

    static Dictionary<string, int> platformKeywords =
        new Dictionary<string, int>();

    [MenuItem("Build/Build Android")]
    static void BuildAndroid()
    {
        variants.Clear();
        shaderKeywords.Clear();
        platformKeywords.Clear();

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.targetGroup = BuildTargetGroup.Android;
        options.target = BuildTarget.Android;
        options.locationPathName = "APK/Test.apk";
        BuildReport report = BuildPipeline.BuildPlayer(options);
        foreach (BuildStep step in report.steps)
        {
            StringBuilder messages = new StringBuilder();
            messages.AppendLine(step.name);
            foreach (BuildStepMessage message in step.messages)
            {
                messages.AppendLine(message.content);
            }
            Debug.Log(messages.ToString());
        }

        foreach (var entry in variants)
        {
            Debug.LogFormat(
                "Shader {0}: has {1} variants",
                entry.Key,
                entry.Value);
        }

        foreach (var entry in shaderKeywords)
        {
            Debug.LogFormat(
                "Keyword {0}: used {1} times",
                entry.Key,
                entry.Value);
        }
    }

    public void OnProcessShader(
        Shader shader,
        ShaderSnippetData snippetData,
        IList<ShaderCompilerData> compilerData)
    {
        foreach (ShaderCompilerData d in compilerData)
        {
            foreach (ShaderKeyword keyword in
                d.shaderKeywordSet.GetShaderKeywords())
            {
                if (shaderKeywords.ContainsKey(keyword.GetKeywordName()))
                {
                    shaderKeywords[keyword.GetKeywordName()]++;
                }
                else
                {
                    shaderKeywords.Add(keyword.GetKeywordName(), 1);
                }
            }
        }

        if (variants.ContainsKey(shader.name))
        {
            variants[shader.name] += compilerData.Count;
        }
        else
        {
            variants.Add(shader.name, compilerData.Count);
        }
    }
}
