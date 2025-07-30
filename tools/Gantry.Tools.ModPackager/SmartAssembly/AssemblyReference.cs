namespace Gantry.Tools.ModPackager.SmartAssembly;

public record AssemblyReference(string Name, string Culture = "neutral", string PublicKeyToken = "null")
{
    public override string ToString() =>
        $"""
        <Assembly AssemblyName="{Name}, Culture={Culture}, PublicKeyToken={PublicKeyToken}">
        	<Merging Merge="1" />
        </Assembly>
        """;
}