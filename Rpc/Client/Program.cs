class Program
{
    public static void Main(string[] args)
    {
        using var rpcClient = new RpcClient();

        var fibonacciArg = args?.FirstOrDefault() ?? "20";
        Console.WriteLine($" [x] Requesting fib({fibonacciArg})");
        var response = rpcClient.Call($"{fibonacciArg}");
        Console.WriteLine(" [.] Got \"{0}\"", response);
    }
}
