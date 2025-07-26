private async Task ThrowExceptionAsync() // Compliant: async method return type is 'Task'
{ throw new InvalidOperationException();
}

public async Task Method()
{
    try
    {
        await ThrowExceptionAsync();
    }
    catch (Exception)
    {
        // The exception is caught here
        throw;
    }
}
