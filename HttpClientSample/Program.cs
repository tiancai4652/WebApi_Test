﻿using HttpClientSample.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HttpClientSample
{
    

    class Program
    {
        static HttpClient client = new HttpClient();

        static void ShowProduct(Device product)
        {
            Console.WriteLine($"Name: {product.Name}" +
                //$"\tPrice: " +
                //$"{product.Price}\tCategory: {product.Category}" +
                $"");
        }

        static async Task<Uri> CreateProductAsync(Device product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/Devices", product);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<Device> GetProductAsync(string path)
        {
            Device product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<Device>();
            }
            return product;
        }

        static async Task<Device> UpdateProductAsync(Device product)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"api/Devices/{product.ID}", product);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            product = await response.Content.ReadAsAsync<Device>();
            return product;
        }

        static async Task<HttpStatusCode> DeleteProductAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/Devices/{id}");
            return response.StatusCode;
        }

        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
        // Update port # in the following line.
        //https://localhost:5001/api/Devices
            client.BaseAddress = new Uri("https://localhost:5001/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a new product
                Device product = new Device
                {
                    ID=Guid.NewGuid().ToString(),
                    Name = "Name"
                };

                var url = await CreateProductAsync(product);
                Console.WriteLine($"Created at {url}");

                // Get the product
                product = await GetProductAsync(url.PathAndQuery);
                ShowProduct(product);

                // Update the product
                Console.WriteLine("Updating price...");
                product.Name = "Name_Update";
                await UpdateProductAsync(product);

                // Get the updated product
                product = await GetProductAsync(url.PathAndQuery);
                ShowProduct(product);

                // Delete the product
                var statusCode = await DeleteProductAsync(product.ID);
                Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}