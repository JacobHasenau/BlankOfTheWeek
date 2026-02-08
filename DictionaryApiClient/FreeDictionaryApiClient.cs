using Domain.Models;
using Domain.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DictionaryApiClient;

public class FreeDictionaryApiClient(IHttpClientFactory _clientFactory) : IDefiner
{
    public async Task<DefinedWord?> DefineWord(string word)
    {
        var client = _clientFactory.CreateClient();

        try
        {
            //Could possibly make this work for multiple languages - just have to change en here.
            var uri = new Uri($"https://api.dictionaryapi.dev/api/v2/entries/en/{word}");
            var result = await client.GetAsync(uri);

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            result.EnsureSuccessStatusCode();
            FreeDictionaryPageDto? dictionaryPage = await GetDictionaryPageFromResult(word, result);

            ArgumentNullException.ThrowIfNull(dictionaryPage);

            var wordDefined = ConvertDictionaryPageToDefinedWord(word, dictionaryPage);
            return wordDefined;
        }
        catch (Exception ex)
        {
            Console.Write($"{ex} was thrown when hitting api to get definition of {word}.");
            return null;
        }
    }

    private static async Task<FreeDictionaryPageDto?> GetDictionaryPageFromResult(string word, HttpResponseMessage result)
    {
        var rawDictionaryPages = await result.Content.ReadAsStringAsync();
        var dictionaryPages = JsonSerializer.Deserialize<List<FreeDictionaryPageDto>>(rawDictionaryPages);
        var dictionaryPage = dictionaryPages?.FirstOrDefault(page => string.Equals(page.word, word, StringComparison.CurrentCultureIgnoreCase));
        return dictionaryPage;
    }

    private static DefinedWord ConvertDictionaryPageToDefinedWord(string word, FreeDictionaryPageDto dictionaryPage)
    {
        IReadOnlyCollection<Meaning> meanings = dictionaryPage.meanings
            .SelectMany(partOfSpeachDefinitions => partOfSpeachDefinitions.definitions
            .Select(definition => new Meaning(partOfSpeachDefinitions.partOfSpeech, definition.definition, definition.example)))
            .ToList();

        IEnumerable<Phonetic> phonetics = dictionaryPage.phonetics
            .Select(phonetic => new Phonetic(phonetic.text));

        var bestFitPhonetic = phonetics.FirstOrDefault(phonetic => !string.IsNullOrWhiteSpace(phonetic.Text) && !phonetic.Text.Any(char.IsDigit));
        var wordDefined = new DefinedWord(word, bestFitPhonetic ?? new Phonetic(word), meanings);
        return wordDefined;
    }
}
