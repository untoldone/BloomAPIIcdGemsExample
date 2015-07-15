using BloomApi;
using BloomApi.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BloomAPIIcdGemsExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //ICD-9 43400 has 13 ICD-10 codes, get all ICD-10 code descriptions
            BloomService service = new BloomService(); // "<API Key HERE as first parameter>");

            // Generates https://www.bloomapi.com/api/search/usgov.hhs.icd_9_gems?key1=icd9&op1=eq&value1=43400
            BloomApiSearchResponse response = service.Search("usgov.hhs.icd_9_gems", new BloomApiSearchOptions
            {
                Terms = new List<BloomApiSearchTerm>
                {
                    new BloomApiSearchTerm{
                        Key = "icd9",
                        Operation = BloomApiSearchOperation.Equals,
                        Value = "43400"
                    }
                }
            });

            IEnumerable<string> tenCodes = response.Result.Select<JObject, string>(mapping => mapping["icd10"].ToString());

            // Generates https://www.bloomapi.com/api/search/usgov.hhs_icd_10_cm?key1=code&op1=eq&value1=<icd-10-code1>&value1=<icd-10-cod2>&value1=...
            // Note, theres a maximum URL length which limits the number of codes you can find this way
            //  You may need to break this out into multiple queries depending on the number of ICD-10 mappings
            BloomApiSearchResponse tenResponse = service.Search("usgov.hhs.icd_10_cm", new BloomApiSearchOptions
            {
                Terms = new List<BloomApiSearchTerm>
                {
                    new BloomApiSearchTerm{
                        Key = "code",
                        Operation = BloomApiSearchOperation.Equals,
                        Values = tenCodes
                    }
                }
            });

            if (tenResponse.Meta.RowCount > 100)
            {
                // BloomAPI only returns 100 items in a single response -- consider adding a loop to the
                // above query to get all the results
                Console.WriteLine("Warning: more codes available than are currently displyed");
            }

            foreach (var result in tenResponse.Result)
            {
                Console.WriteLine(result["long_description"].ToString());
            }

            Console.ReadKey();
        }
    }
}
