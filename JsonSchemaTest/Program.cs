// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;

var sourceFolder = Directory.GetParent(Directory.GetCurrentDirectory());


EvaluationOptions.Default.OutputFormat = OutputFormat.List;
//EvaluationOptions.Default.EvaluateAs = SpecVersion.Draft202012;
var schemaPath = Path.Combine(sourceFolder.FullName, "Schemas");
var testFilePath = Path.Combine(sourceFolder.FullName, "TestFiles");
var files = Directory.GetFiles(schemaPath, "*.schema.json");
foreach (var file in files)
{
    var fileName = Path.GetFileName(file);
    var schema = JsonSchema.FromFile(file);
    SchemaRegistry.Global.Register(schema);
}

var _schema = SchemaRegistry.Global.Get(new System.Uri($"file:///{schemaPath}/root.schema.json")) as JsonSchema;
var doc = JsonNode.Parse(File.ReadAllText($"{testFilePath}/test1.json"));

var result = _schema.Evaluate(doc);
var response = new
{
    IsValid = result.IsValid,
    Errors = result.Details.Where(x => x.HasErrors).SelectMany(x => x.Errors.Values).ToList()
};

var options = new JsonSerializerOptions
{
    WriteIndented = true
};

Console.WriteLine(JsonSerializer.Serialize(response, options));
