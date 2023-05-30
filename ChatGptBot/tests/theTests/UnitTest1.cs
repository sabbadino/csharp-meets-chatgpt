using Azure.AI.OpenAI;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using ChatGptBot.Dtos.Completition;
using ChatGptBot.Dtos.Completition.Controllers;
using ChatGptBot.LangChain;
using ChatGptBot.LangChain.Bricks;
using ChatGptBot.Services;
using ChatGptBot.Services.PlaceHolders;
using ChatGptBot.Settings;
using Microsoft.Extensions.Configuration;


namespace theTests
{
    public class UnitTest1
    {
        [Fact]
        public void TestCosineProximity1()
        {
            var openAiSettings = Options.Create<OpenAiSettings>(
                // defaults should be fine for embedding model 
                new OpenAiSettings()
            );
            var x = new CosineProximityService(openAiSettings);
            var list1 = new List<float> {-1, 1};
            var list2 = new List<float> { 1,- 1 };
            var ret = x.GetProximity(list1, list2);

        }
        //[Fact]
        //public void TestCosineProximity2()
        //{
        //    var openAiSettings = Options.Create<OpenAiSettings>(
        //        // defaults should be fine for embedding model 
        //        new OpenAiSettings()
        //    );
        //    var x = new CosineProximityService(openAiSettings);
        //    var list1 = new List<float> { -1, 1 };
        //    var list2 = new List<float> { 1, -1 };
        //    var ret1 = x.GetProximity(list1, list2);
        //    var ret2 = x.GetProximity2(list1, list2);

        //    ret1.Should().BeApproximately(ret2,0.0001f);

        //    list1 = new List<float> { -1, 1, 5, -1,7 };
        //    list2 = new List<float> { -1, 1, 5, -1, 7 };
        //    ret1 = x.GetProximity(list1, list2);
        //    ret2 = x.GetProximity2(list1, list2);

        //    ret1.Should().BeApproximately(ret2, 0.0001f);
        //}

        [Fact]
        public async void TestSameTextGiveCosineProximityEqualToOne()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<UnitTest1>()
                .Build();
            var text1 = "I am programming in C# against chatGpt";
            var text2 = text1;
            var apiKey = configuration.GetValue<string>("openAiSettings:apiKey");
            var openAiClient = new OpenAIClient(apiKey);
            var openAiSettings = Options.Create<OpenAiSettings>(
                // defaults should be fine for embedding model 
                new OpenAiSettings()
            );
            var embeddingService = new EmbeddingServiceCore(openAiClient, openAiSettings);
            var list1 = await embeddingService.GetTextEmbeddings(text1);
            var list2 = await embeddingService.GetTextEmbeddings(text2);
            var cosineProximityService = new CosineProximityService(openAiSettings);

            var ret = cosineProximityService.GetProximity(list1, list2);
            ret.Should().BeApproximately(1f,0.01f);

        }

        [Fact]
        public async void TestMatchingSameTextShouldReturnMatch()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<UnitTest1>()
                .Build();
            var text = "I am programming in C# against chatGpt";
            // defaults should be fine for embedding model 
            var apiKey = configuration.GetValue<string>("openAiSettings:apiKey");
            var openAiClient = new OpenAIClient(apiKey);
            var openAiSettings = Options.Create<OpenAiSettings>(new OpenAiSettings());

            var embeddingService = new EmbeddingServiceCore(openAiClient, openAiSettings);
            var list1 = await embeddingService.GetTextEmbeddings(text);
            var list2 = await embeddingService.GetTextEmbeddings(text);
            var x = new CosineProximityService(openAiSettings);

            var ret = x.GetClosestMatches(new ProximityRequest
            {
                MaxItems = 3,
                SimilarityThreshold = 0.75f,
                InputEmbedding = list1,
                EmbeddingSet = new List<Embedding> { new Embedding { VectorValues = list2, Id = Guid.NewGuid() } }
            });
            ret.Should().HaveCount(1);

        }

        [Fact]
        public async void TestMatchingSameTextShouldReturnNoMatch()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<UnitTest1>()
                .Build();
            var apiKey = configuration.GetValue<string>("openAiSettings:apiKey");
            var text1 = "I am programming in C# against chatGpt";
            var text2 = "We noticed a new sign-in to your Google Account on a Windows device. If this was you, you don’t need to do anything. If not, we’ll help you secure your account";
            // defaults should be fine for embedding model 
            var openAiClient = new OpenAIClient(apiKey);
            var openAiSettings = Options.Create<OpenAiSettings>(new OpenAiSettings());

            var embeddingService = new EmbeddingServiceCore(openAiClient, openAiSettings);
            var list1 = await embeddingService.GetTextEmbeddings(text1);
            var list2 = await embeddingService.GetTextEmbeddings(text2);
            var x = new CosineProximityService(openAiSettings);

            var ret = x.GetClosestMatches(new ProximityRequest
            {
                MaxItems = 3,
                SimilarityThreshold = 0.75f,
                InputEmbedding = list1,
                EmbeddingSet = new List<Embedding> { new Embedding { VectorValues = list2, Id = Guid.NewGuid() } }
            });
            ret.Should().HaveCount(0);

        }





        [Fact]
        public async void TestVeryDifferentTextGiveCosineProximityLow()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<UnitTest1>()
                .Build();
            var apiKey = configuration.GetValue<string>("openAiSettings:apiKey");
            var text1 = "I am programming in C# against chatGpt";
            var text2 = "This is a beautiful day to let it get away";
            var openAiClient = new OpenAIClient(apiKey);
            var openAiSettings = Options.Create<OpenAiSettings>(
                // defaults should be fine for embedding model 
                new OpenAiSettings()
            );
            var embeddingService = new EmbeddingServiceCore(openAiClient, openAiSettings);
            var list1 = await embeddingService.GetTextEmbeddings(text1);
            var list2 = await embeddingService.GetTextEmbeddings(text2);
            var cosineProximityService = new CosineProximityService(openAiSettings);

            var ret = cosineProximityService.GetProximity(list1, list2);
            ret.Should().BeLessThan(0.75f);

        }

        [Fact]
        public async Task Replace1()
        {
            var b= new LangChainBuilderFactory().Create();
           
           
            var opt = Options.Create(
                new PlaceholdersInformation { LinkRoot= "https://localhost/plugin/" });
            var hr = new HtmlLinkPlaceHolderResolver(opt);
            var srv = new PlaceHolderResolverService(hr);
            var pr = new PlaceHolderResolverBrick(srv);
            b.Add(pr);

            var m = new Mock<ILangChainBrick>();
            b.Add(pr);
            var marker = PlaceHolderResolverService.PlaceHolderMarker;

            var htmlp1 = new HtmlLinkPlaceHolder
            {
                Category = PlaceHolderBase.HTmlLinkCategory,
                LinkCategory = LinkCategory.Booking,
                Value = "12345"
            };
            var body1 = System.Text.Json.JsonSerializer.Serialize(htmlp1);
            var htmlp2 = new HtmlLinkPlaceHolder
            {
                Category = PlaceHolderBase.HTmlLinkCategory,
                LinkCategory = LinkCategory.ShippingInstruction,
                Value = "12345"
            };
            var body2 = System.Text.Json.JsonSerializer.Serialize(htmlp2);

            m.Setup(m => m.Ask(It.IsAny<Question>())).ReturnsAsync(new AnswerToUser
            {
                AnswerFromAi =$"abcd {marker}{body1}{marker} efgh {marker}{body2}{marker} casdcascsac "
            });
            b.Add(m.Object);

            var lc = b.Build();
            var resp = await lc.Ask(new Question
            {
                UserQuestion = new TextWIthTokenCount {Text = "whatever", Tokens=0}
            });
            resp.Should().NotBeNull();
            resp.AnswerFromAi.Should().NotContain(marker);
            
            resp.AnswerFromAi.Should().Contain(opt.Value.LinkRoot + "/booking/12345");
            resp.AnswerFromAi.Should().Contain(opt.Value.LinkRoot + "/ShippingInstructions/12345");
        }

    }
}