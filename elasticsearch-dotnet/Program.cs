﻿using Nest;

namespace elasticsearch_dotnet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var localHostUrl = "http://localhost:9200";
            var defaultIndex = "sametuca-medium";

            var node = new Uri(localHostUrl);
            var settings = new ConnectionSettings(node)
                .DefaultIndex(defaultIndex)
                .DisableDirectStreaming();
            var client = new ElasticClient(settings);

            var document = new MyDocument
            {
                Id = 1,
                Title = "Örnek Belge 1",
                Content = "Elasticsearch medium makalesi için örnek - sametuca"
            };

            var indexResponse = client.IndexDocument(document);

            if (indexResponse.IsValid)
            {
                Console.WriteLine("Belge başarıyla indekslendi.");
            }
            else
            {
                Console.WriteLine("Belge indekslenirken hata oluştu: " + indexResponse.DebugInformation);
            }

            var searchResponse = client.Search<MyDocument>(s => s
                .Index(defaultIndex)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Title)
                        .Query("Elasticsearch medium makalesi için örnek - sametuca")
                    )
                )
            );

            //bir hata olduğunda detaylı bilgi alabiliriz. 
            //var res = searchResponse.DebugInformation;
            //Console.WriteLine(res);

            if (searchResponse.IsValid)
            {
                Console.WriteLine("Arama sonuçları:");
                foreach (var hit in searchResponse.Hits)
                {
                    Console.WriteLine("Id: " + hit.Id);
                    Console.WriteLine("Content: " + hit.Source.Content);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Arama yapılırken hata oluştu: " + searchResponse.DebugInformation);
            }

            Console.ReadLine();
        }
    }
}