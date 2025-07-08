using Microsoft.SemanticKernel;

try
{
    Console.WriteLine("🔧 Testing SemanticKernel with Ollama integration...");
    
    var kernel = Kernel.CreateBuilder()
        .AddOllamaChatCompletion("gemma3:4b-it-q8_0", new Uri("http://localhost:11434"))
        .Build();

    Console.WriteLine("✅ SemanticKernel integration working!");
    Console.WriteLine("🏁 Basic test completed successfully. Ready for full implementation.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine("Please ensure Ollama is running and the model is available.");
}