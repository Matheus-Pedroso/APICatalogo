namespace APICatalogo.Services;

public class MyService : IMyServices
{
    public string Salutation(string nome)
    {
        return $"Bem-Vindo, {nome} \n\n {DateTime.UtcNow}";
    }
}
