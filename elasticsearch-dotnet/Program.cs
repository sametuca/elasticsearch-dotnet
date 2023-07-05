using Nest;

namespace elasticsearch_dotnet
{
    public class MyDocument
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var node = new Uri("http://localhost:9200"); // Elasticsearch sunucusunun URL'si
            var settings = new ConnectionSettings(node)
                .DefaultIndex("your-default-index-name")
                .DisableDirectStreaming();// Varsayılan indeks adını belirtin
            var client = new ElasticClient(settings);

            var document = new MyDocument
            {
                Id = 1,
                Title = "Örnek Belge",
                Content = "Bu bir Elasticsearch örneği."
            };

            // Belgeyi indeksleme
            var indexResponse = client.IndexDocument(document);

            if (indexResponse.IsValid)
            {
                Console.WriteLine("Belge başarıyla indekslendi.");
            }
            else
            {
                Console.WriteLine("Belge indekslenirken hata oluştu: " + indexResponse.DebugInformation);
            }

            // Belgeyi sorgulama
            var searchResponse = client.Search<MyDocument>(s => s
                .Index("your-default-index-name") // Sorgulama yapmak istediğiniz indeks adı
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Title)
                        .Query("Elasticsearch")
                    )
                )
            );
            var res = searchResponse.DebugInformation;
            Console.WriteLine(res);

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