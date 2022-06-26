using System.Data;
using Discord.Addons.Interactive;
using Discord.Commands;

namespace JokeBot.Modules;

public class Commands : InteractiveBase
{
    private const string _apiUrl = "https://backend-omega-seven.vercel.app/api/getjoke";

    [Command("joke", RunMode = RunMode.Async)]
    public async Task Joke()
    {
        string question = "";
        string punchline = "";

        foreach (DataRow item in RetrieveData(_apiUrl).Result.Rows)
        {
            question = item["question"].ToString();
            punchline = item["punchline"].ToString();
        }

        await ReplyAsync(question);
        var response = await NextMessageAsync();
        await ReplyAsync(punchline);
    }

    public async Task<DataTable> RetrieveData(string _apiUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(_apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(_apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var table = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(data);
                return table;
            }
        }
        return null;
    }
}