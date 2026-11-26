Console.WriteLine("Hello, World!");

Console.WriteLine("Type 'exit' to close.");
int Count = 0;

while (true)
{
    Console.Write("You: ");

    string? input = Console.ReadLine();

    if (input?.ToLower() == "exit")
        break;

    Console.WriteLine($"You entered: {input}");
    Count++;
    Console.WriteLine($"Count Chat: {Count}");
}
