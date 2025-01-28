using HeroDex.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HeroDex.Services
{
    public class HeroesService
    {
        private readonly HttpClient _httpClient;

        public HeroesService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(@"http://188.245.250.41:8080");
        }


        public async Task<Hero> GetHeroByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"/heroes/get?id={id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Hero>(content);
        }

        public async Task<List<Hero>> GetAllHeroesAsync(string type = null, string name = null)
        {
            var query = string.Empty;
            if (!string.IsNullOrEmpty(type) || !string.IsNullOrEmpty(name))
            {
                query = $"?type={type}&name={name}";
            }

            var response = await _httpClient.GetAsync(@$"/heroes/list{query}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Hero>>(content);
        }   

        public async Task<ImageSource> GetHeroImageAsync(string id)
        {
            return await GetImageSourceFromUrl($"/heroes/get/{id}/image");
        }

        public async Task CreateHeroAsync(Hero hero, ImageSource imageSource)
        {
            var multipartContent = new MultipartFormDataContent();

            if (imageSource != null)
            {
                // Получаем поток данных из ImageSource
                var stream = await GetStreamFromImageSourceAsync(imageSource);
                if (stream != null)
                {
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    multipartContent.Add(streamContent, "image", "image.jpg"); // Имя файла по умолчанию
                }
            }

            var heroData = JsonSerializer.Serialize(hero);
            multipartContent.Add(new StringContent(heroData), "data");

            var response = await _httpClient.PostAsync("/heroes/create", multipartContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateHeroAsync(Hero hero, ImageSource imageSource)
        {
            var multipartContent = new MultipartFormDataContent();

            if (imageSource != null)
            {
                var stream = await GetStreamFromImageSourceAsync(imageSource);
                if (stream != null)
                {
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    multipartContent.Add(streamContent, "image", "image.jpg");
                }
            }

            var heroData = JsonSerializer.Serialize(hero);
            multipartContent.Add(new StringContent(heroData), "data");

            var response = await _httpClient.PatchAsync("/heroes/update", multipartContent);
            response.EnsureSuccessStatusCode();
        }


        public async Task DeleteHeroAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"/heroes/delete?id={id}");
            response.EnsureSuccessStatusCode();
        }

        private async Task<Stream> GetStreamFromImageSourceAsync(ImageSource imageSource)
        {
            if (imageSource is FileImageSource fileImageSource)
            {
                // Локальный файл
                if (!string.IsNullOrEmpty(fileImageSource.File) && File.Exists(fileImageSource.File))
                {
                    return File.OpenRead(fileImageSource.File);
                }
            }
            else if (imageSource is StreamImageSource streamImageSource)
            {
                return await streamImageSource.Stream(CancellationToken.None);
            }

            return null;
        }


        private async Task<ImageSource> GetImageSourceFromUrl(string imageUrl)
        {
            try
            {
                var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
                return ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
