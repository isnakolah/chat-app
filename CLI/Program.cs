using var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://1rur03flf4.execute-api.us-east-1.amazonaws.com/Prod/")
};

async Task<string> Add(int x, int y)
{
    var result = await httpClient.GetAsync($"calculator/add/{x}/{y}");

    return await result.Content.ReadAsStringAsync();
}

static bool GetInputFromConsole(out int value)
{
    if (int.TryParse(ReadLine(), out value))
        return true;

    WriteLine("Your input is invalid, try again");

    return false;
}

while (true)
{
    firstNumber:
    Write("\nFirst number: ");

    if (!GetInputFromConsole(out var x))
        goto firstNumber;

    secondNumber:
    Write("Second number: ");

    if (!GetInputFromConsole(out var y))
        goto secondNumber;

    var total = await Add(x, y);

    WriteLine($"The total is {total}");
}