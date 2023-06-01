namespace Practical2_Kanbanize_API___Valkan
{
    using RestSharp;
    using System.Net;
    using System.Text.Json.Serialization;
    using System.Text.Json;
    using RestSharp.Authenticators;
    using System;

    
    public class API_CardFuncionalityTests
    {
        private RestClient client;
        private const string username = "apikey";
        private const string password = "PKgy6ToghixeLCRgq61NNclBkEVOzjgny7UzptYV";
        private const string mainUrl = "https://valkangeorgiev.kanbanize.com";
        private const string secondaryUrl = "api/v2/cards";

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(mainUrl);
        }

        [Test]
        public void CreateNewCardAndCheckParameters()
        {
            Body newBody = CreateBody(1,2,1,0,100,"NewTestCard", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var cardId = responseBody.data[0].card_id;

            var newRequest = CreateRequestGet(cardId);
            var newResponse = client.Execute(newRequest);
            var responseNewBody = JsonSerializer.Deserialize<CardResponseObject>(newResponse.Content);

            Assert.That(newResponse.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(newResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseNewBody.data.lane_id, Is.EqualTo(1));
            Assert.That(responseNewBody.data.column_id, Is.EqualTo(2));
            Assert.That(responseNewBody.data.workflow_id, Is.EqualTo(1));
            Assert.That(responseNewBody.data.position, Is.EqualTo(0));
            Assert.That(responseNewBody.data.priority, Is.EqualTo(100));
            Assert.That(responseNewBody.data.title, Is.EqualTo("NewTestCard"));
            Assert.That(responseNewBody.data.color, Is.EqualTo("F8F32B".ToLower()));
        }
        [Test,Order(1)]
        public void TryToCreateCardWithTextLineId()
        {
            var reqBody = new { lane_id = "one", column_id = 1, title = "Test Title" };
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request" +
                " body did not pass validation."));
            Assert.That(responseError.error.details.lane_id[0], Is.EqualTo("The value must be an integer."));
        }
        [Test,Order(2)]
        public void Test_ÒryToCrateCardWithLaneIdEqualToZero()
        {
            Body body = CreateBody(0, 1, 1, 0, 100, "TestLaneId", "F8F32B");
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request" +
                " body did not pass validation."));
            Assert.That(responseError.error.details.lane_id[0], Is.EqualTo("The value must be a positive number."));
        }
        [Test, Order(3)]
        public void ÒryToCrateCardWithDifferentWorkflowOfLaneIdAndColumnId()
        {
            Body body = CreateBody(2, 1, 1, 0, 100, "Test title", "F8F32B");
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<int>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The lane and column of " +
                "a card must be in the same workflow. The lane with id 2 and the column " +
                "with id 1 are not."));
        }

        [Test, Order(4)]
        public void TryToCreateCardWithDoubleLineId()
        {
            var reqBody = new {lane_id = 1.1, column_id = 1, title = "Test Title"};
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request" +
                " body did not pass validation."));
            Assert.That(responseError.error.details.lane_id[0], Is.EqualTo("The value must be an integer."));
        }
        [Test, Order(5)]
        public void ÒryToCrateCardWithMissingLaneId()
        {
            Body body = CreateBody(null, 1, 1, 0, 100, "New Title", "F8F32B");
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<int>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("Please provide a lane_id for the new " +
                "card with reference CCR or have it copied from an " +
                "existing card by using card_properties_to_copy."));
        }
        [Test, Order(6)]
        public void ÒryToCrateCardWithNegativeLaneId()
        {
            Body body = CreateBody(-1, 1, 1, 0, 100, "New Title", "F8F32B");
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.lane_id[0], Is.EqualTo("The value must be a positive number."));
        }
        [Test, Order(7)]
        public void Test_ÒryToCrateCardWithLaneIdEqualsFour()
        {
            Body body = CreateBody(4, 1, 1, 0, 100, "New Title", "F8F32B");
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<int>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("A lane with id 4 does not exist."));
        }
        [Test, Order(8)]
        public void TryToCreateCardWithTextColumnId()
        {
            var reqBody = new { lane_id = 1, column_id = "one", title = "Test Title" };
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request" +
                " body did not pass validation."));
            Assert.That(responseError.error.details.column_id[0], Is.EqualTo("The value must be an integer."));
        }
        [Test, Order(9)]
        public void ÒryToCrateCardWithMissingColumnId()
        {
            Body body = CreateBody(1, null, 1, 0, 100, "New Title", "F8F32B");
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<int>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("Please provide a column_id for the new " +
                "card with reference CCR or have it copied from an " +
                "existing card by using card_properties_to_copy."));
        }
        [Test, Order(10)]
        public void ÒryToCrateCardWithNegativeColumnId()
        {
            Body body = CreateBody(1, -1, 1, 0, 100, "New Title", "F8F32B");
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.column_id[0], Is.EqualTo("The value must be a positive number."));
        }

        [Test, Order(11)]
        public void TryToCreateCardWithDoubleColumnId()
        {
            var reqBody = new { lane_id = 1, column_id = 1.1, title = "Test Title" };
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request" +
                " body did not pass validation."));
            Assert.That(responseError.error.details.column_id[0], Is.EqualTo("The value must be an integer."));
        }
        [Test, Order(12)]
        public void Test_ÒryToCrateCardWithColumnIdEqualsSix()
        { 
            Body body = CreateBody(1, 6, 1, 0, 100, "New Title", "F8F32B");
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<int>>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            Assert.That(responseError.error.message, Is.EqualTo("The lane and column of a card" +
                " must be in the same workflow. The lane with id 1 and the column with id 6 are not."));
        }
        [Test, Order(13)]
        public void ÒryToCrateCardWithColumnIdEqualsFive()
        {
            Body newBody = CreateBody(1, 5, 1, 0, 100, "NewTestCard", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].lane_id, Is.EqualTo(1));
            Assert.That(responseBody.data[0].column_id, Is.EqualTo(5));
            Assert.That(responseBody.data[0].workflow_id, Is.EqualTo(1));
            Assert.That(responseBody.data[0].position, Is.EqualTo(0));
            Assert.That(responseBody.data[0].priority, Is.EqualTo(100));
            Assert.That(responseBody.data[0].title, Is.EqualTo("NewTestCard"));
            Assert.That(responseBody.data[0].color, Is.EqualTo("F8F32B".ToLower()));
        }
        [Test, Order(14)]
        public void ÒryToCrateCardWithColumnIdEqualsZero()
        {
            Body newBody = CreateBody(1, 0, 1, 0, 100, "NewTestCard", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request" +
                " body did not pass validation."));
            Assert.That(responseError.error.details.column_id[0], Is.EqualTo("The value must be a positive number."));
        }
        [Test, Order(15)]
        public void ÒryToCrateCardWithMissingWorkflowId()
        {
            Body newBody = CreateBody(1, 1, null, 0, 100, "WorkflowTest", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].workflow_id, Is.EqualTo(1));
        }
        [Test, Order(16)]
        public void ÒryToCrateCardWithNegativeWorkflowId()
        {
            Body newBody = CreateBody(1, 1, -1, 0, 100, "WorkflowTest", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].workflow_id, Is.EqualTo(1));
        }
        [Test, Order(17)]
        public void ÒryToCrateCardOnTheSecondWorkflow()
        {
            Body newBody = CreateBody(2, 9, 2, 0, 100, "WorkflowTest", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].workflow_id, Is.EqualTo(2));
        }
        [Test, Order(18)]
        public void ÒryToCrateCardWithMissingTitle()
        {
            Body newBody = CreateBody(1, 1, 1, 0, 100, null, "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].title, Is.EqualTo(""));   
        }
        [Test, Order(19)]
        public void ÒryToCrateCardWithTitleEqualsEmptyString()
        {
            Body newBody = CreateBody(1, 1, 1, 0, 100, "", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].title, Is.EqualTo(""));
               
        }

        [Test, Order(20)]
        public void ÒryToCrateCardWithTitleEqualsTwoHundredDifferentSymbols()
        {
            Body newBody = CreateBody(1, 1, 1, 0, 100, "FpYbfhmPKLDwqw85I4eoNflo" +
                "bFrmwxWodhB8erY2ucpWLIYqOZBon58LJHknIq4dxyUv0FvvKtFfmK5xoy6r2G" +
                "S3jRZg9h59DqRb7vXinVYZCwySqXfjKBrdrsrDc7Bwu7IkPkQ7Zs8p2LfRjtR9" +
                "KBJ98LqhHn0ThEjI3TGgAOVxIr6qQ9gf0P8BbcjxXbctarJnfiVx", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);
        
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
               Assert.That(responseBody.data[0].title, Is.EqualTo("FpYbfhmPKLDwqw85I4eoNflo" +
                "bFrmwxWodhB8erY2ucpWLIYqOZBon58LJHknIq4dxyUv0FvvKtFfmK5xoy6r2G" +
                "S3jRZg9h59DqRb7vXinVYZCwySqXfjKBrdrsrDc7Bwu7IkPkQ7Zs8p2LfRjtR9" +
                "KBJ98LqhHn0ThEjI3TGgAOVxIr6qQ9gf0P8BbcjxXbctarJnfiVx"));
        }
        [Test, Order(21)]
        public void TryToCreateCardWithIntegerTitle()
        {
            var reqBody = new { lane_id = 1, column_id = 1, title = 1 };
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].title, Is.EqualTo("1"));
        }
        [Test, Order(22)]
        public void TryToCreateCardWithNegativeTitle()
        {
            var reqBody = new { lane_id = 1, column_id = 1, title = -1 };
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].title, Is.EqualTo("-1"));
        }
        [Test, Order(23)]
        public void TryToCreateCardWithDoubleTitle()
        {
            var reqBody = new { lane_id = 1, column_id = 1, title = 1.1 };
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].title, Is.EqualTo("1.1"));
        }
        [Test, Order(24)]
        public void Test_ÒryToCrateCardWitInvalidPosition()
        {
            Body newBody = CreateBody(1, 1, 1, -1, 100, "", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.position[0], Is.EqualTo("The value must be " +
                "greater than or equal to zero."));
        }
        [Test, Order(25)]
        public void TryToCreateCardWithStringPosition()
        {
            var reqBody = new { lane_id = 1, column_id = 1, position = "position"};
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.position[0], Is.EqualTo("The value must be an integer."));
        }
        [Test, Order(26)]
        public void TryToCreateCardWithDoublePosition()
        {
            var reqBody = new { lane_id = 1, column_id = 1, position = 1.1};
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.position[0], Is.EqualTo("The value must be an integer."));
        }
        // Òîçè òåñò ùå ìèíå ñàìî ïúðâèÿò ïúò, êîãàòî êàðòàòà ñå ñúçäàâà, çàòîâà â òåñòà ùå èçòðèåì ñàìàòà
        // êàðòà, çà äà ìèíàâà âñåêè ïúò. Àêî íå ÿ èçòðèåì, ñëåäâàùèÿò ïúò, êîãàòî ïóñíåì òåñòà, ïîçèöèÿòà
        // ùå å ðàâíà íà 1, ñëåä òîâà íà 2 è òàêà â çàâèñèìîñò îò áðîÿ íà êàðòèòå
        [Test, Order(27)]
        public void TryToCreateCardWithMissingPositionWhenThereIsNoOtherCardInTheColumn()
        {
            // Ñúçäàâàìå íîâà êàðòà, êîÿòî å ñ ïîçèöèÿ 0

            Body newBody = CreateBody(2, 7, 1, null, 100, "TestPosition", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);
            var cardId = responseBody.data[0].card_id;

            //Ïðîâåðÿâàìå, ÷å å ñ ïîçèöèÿ 0

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].position, Is.EqualTo(0));

            //Èçòðèâàìå ÿ, çà äà ìîæå ñëåäâàùèÿò ïúò, êîãàòî ïóñíåì òåñòà, îòíîâî äà å íà ïîçèöèÿ 0

            var deleteRequest = new RestRequest(secondaryUrl + $"/{cardId}", Method.Delete);
            deleteRequest.AddHeader(username, password);
            var cardResponse = client.Execute(deleteRequest);
            Assert.That(cardResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
        [Test, Order(28)]
        public void TryToCreateCardWithMissingPositionWhenThereIsOtherCardsInTheColumn()
        {
            Body newBody = CreateBody(1, 1, 1, null, 100, "TestPosition", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);
            var cardPosition = responseBody.data[0].position;

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].position, Is.EqualTo(cardPosition));
        }

        [TestCase("F8"), Order(29)]
        [TestCase("F8F32BB")]
        public void TryToCreateCardWithColorWithIncorectSymbols(string color)
        {
            Body newBody = CreateBody(1, 1, 1, 0, 100, "TestColor", color);
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.color[0], Is.EqualTo("The value must be a " +
                "valid 6-character or 3-character color string."));
        }
        [TestCase(-1),Order(30)]
        [TestCase(0)]
        public void TryToCreateCardWithColorWithIncorectIntegerSymbols(int color)
        {
            var reqBody = new { lane_id = 1, column_id = 1, color = color };
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.color[0], Is.EqualTo("The value must be a " +
                "valid 6-character or 3-character color string."));
        }

        [Test, Order(31)]
        public void TryToCreateCardWithColorWithDoubleNumber()
        {
            var reqBody = new { lane_id = 1, column_id = 1, color = 1.1 };
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.color[0], Is.EqualTo("The value must be a " +
                "valid 6-character or 3-character color string."));
        }
        [Test, Order(32)]
        public void TryToCreateCardWithIntegerColorWithÒhreeDigits()
        {
            var reqBody = new { lane_id = 1, column_id = 1, position = 0, color = 123 };
            var request = new RestRequest(secondaryUrl, Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);
          
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].color, Is.EqualTo("112233"));
        }
        [Test, Order(33)]
        public void TryToCreateCardWithMissingColor()
        {
            Body newBody = CreateBody(1, 1, 1, 0, 100, "TestColor", null);
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);
            var color = responseBody.data[0].color;

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].color, Is.EqualTo(color));
        }

        [Test, Order(34)]
        public void UpdateCard()
        {
            var request = CreatedCard();
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var cardId = responseBody.data[0].card_id;

            Body bodyRequest = new Body();
            Body body = CreateBody(1, 3, 1, 0, 100, "NewTestCard", "#F8F32B");
            var newRequest = CreateRequestPatch(body, cardId);
            var newResponse = client.Execute(newRequest);
            var responseNewBody = JsonSerializer.Deserialize<CardResponse>(newResponse.Content);

            Assert.That(newResponse.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(newResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseNewBody.data[0].column_id, Is.EqualTo(3));
           
        }
        [Test, Order(35)]
        public void TryToUpdateCardWithInvalidLaneId()
        {
            Body body = CreateBody(2, 1, 1, 0, 100, "UPDATE CARD", "#F8F32B");
            var newRequest = CreateRequestPatch(body, 71);
            var newResponse = client.Execute(newRequest);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<int>>(newResponse.Content);

            Assert.That(newResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The lane and column of a card must be" +
                " in the same workflow. The lane with id 2 and the column with id 1 are not."));
        }
        [Test, Order(36)]
        public void UpdateCardWithInvalidColumnId()
        {
            Body body = CreateBody(1, 10, 1, 0, 100, "UPDATE CARD", "#F8F32B");
            var newRequest = CreateRequestPatch(body, 71);
            var newResponse = client.Execute(newRequest);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<int>>(newResponse.Content);

            Assert.That(newResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The lane and column of a card must be " +
                "in the same workflow. The lane with id 1 and the column with id 10 are not."));
        }
        [Test, Order(37)]
        public void TryToUpdateCardWithIncorectCardId()
        {
            Body body = CreateBody(1, 1, 1, 0, 100, "UPDATE CARD", "F8F32B");
            var newRequest = CreateRequestPatch(body, 2000);
            var newResponse = client.Execute(newRequest);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<int>>(newResponse.Content);

            Assert.That(newResponse.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(newResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(responseError.error.message, Is.EqualTo("A card with id 2000 does not exist."));
        }
        [Test, Order(38)]
        public void UpdateCardWithInvalidPosition()
        {
            Body newBody = CreateBody(1, 1, 1, 300, 100, "TestPosition", "F8F32B");
            var request = CreateRequestPatch(newBody, 71);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);
            var cardPosition = responseBody.data[0].position;

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].position, Is.EqualTo(cardPosition));
        }
        [Test, Order(39)]
        public void UpdateCardWithInvalidColor()
        {
            Body newBody = CreateBody(1, 1, 1, 0, 100, "TestPosition", "12");
            var request = CreateRequestPatch(newBody, 71);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.color[0], Is.EqualTo("The value must be a " +
                "valid 6-character or 3-character color string."));
        }


        [TestCase(1, 1, 0, 100, "TestTitle", "F8F32B"), Order(40)]
        [TestCase(1, 1, 0, 100, "NewTestTitle", "F8F32B")]
        [TestCase(1, 2, 0, 100, "TestTitle", "F8F32B")]
        [TestCase(1, 5, 0, 250, "TestTitle", "123456")]
        public void TryToUpdateCardWithCorrectData(int lane_id, int column_id, int position,int priority, string title, string color)
        {
            Body body = CreateBody(lane_id, column_id, 1, position, priority, title, color);
            var newRequest = CreateRequestPatch(body, 71);
            var newResponse = client.Execute(newRequest);
            var responseNewBody = JsonSerializer.Deserialize<CardResponse>(newResponse.Content);

            Assert.That(newResponse.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(newResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseNewBody.data[0].column_id, Is.EqualTo(column_id));
            Assert.That(responseNewBody.data[0].position, Is.EqualTo(position));
            Assert.That(responseNewBody.data[0].priority, Is.EqualTo(priority));
            Assert.That(responseNewBody.data[0].title, Is.EqualTo(title));
            Assert.That(responseNewBody.data[0].color, Is.EqualTo(color.ToLower()));
        }

        [Test, Order(41)]
        public void CreateCardSubtask()
        {
            Body subtaskBody = CreateSubtaskBody(0, "new subtask");
            var request = CreateRequestPost(subtaskBody,secondaryUrl + "/32/subtasks");
            var response = client.Execute(request);
            var responseSubtaskBody = JsonSerializer.Deserialize<CardResponseObject>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseSubtaskBody.data.description, Is.EqualTo("new subtask"));
            Assert.That(responseSubtaskBody.data.position, Is.EqualTo(0)); 
        }
        [Test, Order(42)]
        public void CreateCardSubtaskWithoutDescription()
        {
            Body subtaskBody = CreateSubtaskBody(0, null);
            var request = CreateRequestPost(subtaskBody, secondaryUrl + "/32/subtasks");
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.description[0], Is.EqualTo("This property is required."));
        }
        [Test, Order(43)]
        public void CreateCardSubtaskWithIncorectOwnerId()
        {
            var reqBody = new { description = "NewSubtaskTest", owner_user_id = 1};
            var request = new RestRequest(secondaryUrl + "/32/subtasks", Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<string>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The owner of" +
                " the subtask with id  cannot be set because the user with id" +
                " 1 is not assigned to the board with id 1."));
        }
        [Test, Order(44)]
        public void CreateCardSubtaskWithNegativeOwnerId()
        {
            var reqBody = new { description = "NewSubtaskTest", owner_user_id = -1 };
            var request = new RestRequest(secondaryUrl + "/32/subtasks", Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in " +
                "the request body did not pass validation."));
            Assert.That(responseError.error.details.owner_user_id[0], Is.EqualTo("The value must be" +
                " a positive number."));
        }
        [Test, Order(45)]
        public void CreateCardSubtaskWithDoubleOwnerId()
        {
            var reqBody = new { description = "NewSubtaskTest", owner_user_id = 1.1 };
            var request = new RestRequest(secondaryUrl + "/32/subtasks", Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.owner_user_id[0], Is.EqualTo("The value must be an integer."));
        }
        [Test, Order(46)]
        public void CreateCardSubtaskOfMissingCard()
        {
            var reqBody = new { description = "NewSubtaskTest", owner_user_id = 1 };
            var request = new RestRequest(secondaryUrl + "/3000/subtasks", Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorResponse<string>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(responseError.error.message, Is.EqualTo("A card with id 3000 does not exist."));
        }
        [Test, Order(47)]
        public void CreateCardSubtaskWithDoubleDescription()
        {
             var reqBody = new { description = 1.1 };
            var request = new RestRequest(secondaryUrl + "/32/subtasks", Method.Post);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseSubtaskBody = JsonSerializer.Deserialize<CardResponseObject>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseSubtaskBody.data.description, Is.EqualTo("1.1")); 
        }

        [Test, Order(48)]
        public void CreateCardSubtaskWithoutPosition()
        {
            Body subtaskBody = CreateSubtaskBody(null, "TestSubtaskDescription");
            var request = CreateRequestPost(subtaskBody, secondaryUrl + "/32/subtasks");
            var response = client.Execute(request);
            var responseSubtaskBody = JsonSerializer.Deserialize<CardResponseObject>(response.Content);
            var positionNumber = responseSubtaskBody.data.position;

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseSubtaskBody.data.description, Is.EqualTo("TestSubtaskDescription"));
            Assert.That(responseSubtaskBody.data.position, Is.EqualTo(positionNumber));
        }

        [Test, Order(49)]
        public void DeleteCard()
        {
            Body newBody = CreateBody(1, 2, 1, 0, 100, "CardforDelete", "#F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var cardId = responseBody.data[0].card_id;

            var deleteRequest = new RestRequest(secondaryUrl + $"/{cardId}", Method.Delete);
            deleteRequest.AddHeader(username, password);
            var cardResponse = client.Execute(deleteRequest);
            Assert.That(cardResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
        [Test, Order(50)]
        public void TryToDeleteMissingCard()
        {
            var deleteRequest = new RestRequest(secondaryUrl + $"/3000", Method.Delete);
            deleteRequest.AddHeader(username, password);
            var cardResponse = client.Execute(deleteRequest);
            Assert.That(cardResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }


        [Test, Order(51)]
        public void TryToEditCardSizeWithIncorectCardId()
        {
            Body body = CreateSizeBody(1, 1, 1, "TestCardSize");
            var request = CreateRequestPatch(body, 3000);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<ErrorResponse<string>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(responseBody.error.message, Is.EqualTo("A card with id 3000 does not exist."));
        }
        [Test, Order(52)]
        public void TryToEditCardWithSizeEqualsZero()
        {
            Body body = CreateSizeBody(1, 1, 0, "TestCardSize");
            var request = CreateRequestPatch(body, 60);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseBody.error.message, Is.EqualTo("The parameters in the " +
                "request body did not pass validation."));
            Assert.That(responseBody.error.details.size[0], Is.EqualTo("The value must" +
                " be a positive number."));
        }
        [Test, Order(53)]
        public void TryToEditCardWithStringSize()
        {
            var reqBody = new { lane_id = 1, column_id = 1, size = "s", title = "TestCardSize" };
            var request = new RestRequest(secondaryUrl +"/60", Method.Patch);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseBody.error.message, Is.EqualTo("The parameters in the " +
                "request body did not pass validation."));
            Assert.That(responseBody.error.details.size[0], Is.EqualTo("The value must be" +
                " a floating-point number."));
        }
        [Test, Order(54)]
        public void TryToEditCardWithSizeWithSpecialSymbols()
        {
            var reqBody = new { lane_id = 1, column_id = 1, size = "@!#!$%^&", title = "TestCardSize" };
            var request = new RestRequest(secondaryUrl + "/60", Method.Patch);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseBody.error.message, Is.EqualTo("The parameters in the " +
                "request body did not pass validation."));
            Assert.That(responseBody.error.details.size[0], Is.EqualTo("The value must be" +
                " a floating-point number."));
        }
        [Test, Order(55)]
        public void TryToEditCardWithSizeWithNegativeNumber()
        {
            var reqBody = new { lane_id = 1, column_id = 1, size = -1, title = "TestCardSize" };
            var request = new RestRequest(secondaryUrl + "/60", Method.Patch);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseBody.error.message, Is.EqualTo("The parameters in the " +
                "request body did not pass validation."));
            Assert.That(responseBody.error.details.size[0], Is.EqualTo("The value must be a positive number."));
        }
        [Test, Order(56)]
        public void TryToEditCardWithSizeEqualsDoubleNumber()
        {
            var newBody = new { lane_id = 1, column_id = 1, size = 1, title = "NEW TEST CARD SIZE" };
            var newRequest = new RestRequest(secondaryUrl , Method.Post);
            newRequest.AddParameter("application/json", newBody, ParameterType.RequestBody);
            newRequest.AddHeader(username, password);
            var newResponse = client.Execute(newRequest);
            var newResponseBody = JsonSerializer.Deserialize<CardResponse>(newResponse.Content);

            var cardId = newResponseBody.data[0].card_id;

            var reqBody = new { lane_id = 1, size = 2.5, title = "TestCardSize" };
            var request = new RestRequest(secondaryUrl + $"/{cardId}", Method.Patch);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<SizeResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].size, Is.EqualTo(2.5));
        }


        [Test, Order(57)]
        public void TryToCreateCardWithDeadlineBefore1970()
        {
            Body body = new Body
            {
                column_id = 1,
                lane_id = 1,
                deadline = "1950-03-10T13:57:03.714Z",
                title = "TestCardDeadline"
            };
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseBody.error.details.deadline[0], Is.EqualTo("The date and time must be " +
                "after 1970-01-01 00:00:00."));
        }
        [TestCase("12 of March"),Order(58)]
        [TestCase("12.03.2023")]
        [TestCase("12-03-2023")]
        [TestCase("1950.03.10T13:57:03.714Z")]
        public void TryToCreateCardWithInvalidTextDeadline(string deadline)
        {
            Body body = new Body
            {
                column_id = 1,
                lane_id = 1,
                deadline = deadline,
                title = "TestCardDeadline"
            };
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseBody.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseBody.error.details.deadline[0], Is.EqualTo("The value must contain" +
                " a valid date and time in the ISO 8601 format including a time zone designator."));
        }

        [TestCase("1970-03-10T00:00:01Z"),Order(59)]
        [TestCase("2038-01-01T00:00:00Z")]
        public void TryToCreateCardWithValidDeadline(string deadline)
        {
            Body body = new Body
            {
                column_id = 1,
                lane_id = 1,
                deadline = deadline,
                title = "TestCardDeadline"
            };
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].deadline, Is.EqualTo(deadline));     
        }

        [Test, Order(60)]
        public void TryToCreateCardWithInvalidDeadline()
        {
            Body body = new Body
            {
                column_id = 1,
                lane_id = 1,
                deadline = "2038-01-01T00:00:01Z",
                title = "TestCardDeadline"
            };
            var request = CreateRequestPost(body, secondaryUrl);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseBody.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseBody.error.details.deadline[0], Is.EqualTo("The date and time must be" +
                " before 2038-01-01 00:00:00."));
        }
        [Test, Order(61)]
        public void TryToCreateSubtaskIntoCard()
        {

            Body subtaskBody = CreateSubtaskBody(0, "NOV SUBTASK");
            var subtaskRequest = CreateRequestPost(subtaskBody, secondaryUrl + "/19/subtasks");
            var subtaskResponse = client.Execute(subtaskRequest);
            var responseSubtaskBody = JsonSerializer.Deserialize<CardResponseObject>(subtaskResponse.Content);

            Assert.That(subtaskResponse.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(subtaskResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var subtaskId = responseSubtaskBody.data.subtask_id;
            var links_to_existing_cards_to_add_or_updateObject = new
            {
                linked_card_id = 111,
                link_type = "child"
            };
            object[] links_to_new_cards_to_add_Array = { links_to_existing_cards_to_add_or_updateObject };
            var subtasks_to_convert_into_cards_Object = new
            {
                subtask_id = subtaskId,
                lane_id = 1,
                column_id = 1,
                title = "CONVERT TO CARD",
                links_to_existing_cards_to_add_or_update = links_to_new_cards_to_add_Array
            };
            object[] subtasks_to_convert_into_cards = { subtasks_to_convert_into_cards_Object };
            var reqBody = new { subtasks_to_convert_into_cards };
            var reqBodyJson = JsonSerializer.Serialize(reqBody);

            var request = new RestRequest(secondaryUrl + "/19", Method.Patch);
            request.AddParameter("application/json", reqBodyJson, ParameterType.RequestBody);
            request.AddHeader(username, password);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

           var getRequest = new RestRequest(secondaryUrl + "/111/LinkedCards", Method.Get);
           getRequest.AddHeader(username, password);
           getRequest.Timeout = 2000;
           var getResponse = client.Execute(getRequest);
           var responseNewBody = JsonSerializer.Deserialize<CardResponse>(getResponse.Content);

           Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
           Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
           Assert.That(responseNewBody.data[0].link_type, Is.EqualTo("parent"));

        }

        // Îïèòàõ ñå äà ñúçäàì òåñò, íî íå ìè ñå ïîëó÷èõà íåùàòà, êàòî öÿëî òàçè ÷àñò ñ êîíâåíòèðàíåòî íà ñúáòàñêà
        // äîñòà ìå çàòðóäíè.

      // [Test]
      // public void TryToCreateSubtaskWithMissingLinkType()
      // {
      //
      //     Body subtaskBody = CreateSubtaskBody(0, "NOV SUBTASK TEST");
      //     var subtaskRequest = CreateRequestPost(subtaskBody, secondaryUrl + "/19/subtasks");
      //     var subtaskResponse = client.Execute(subtaskRequest);
      //     var responseSubtaskBody = JsonSerializer.Deserialize<CardResponseObject>(subtaskResponse.Content);
      //
      //     Assert.That(subtaskResponse.ContentType.StartsWith("application/json"), Is.True);
      //     Assert.That(subtaskResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
      //
      //     var subtaskId = responseSubtaskBody.data.subtask_id;
      //
      //     var links_to_existing_cards_to_add_or_updateObject = new
      //     {
      //         linked_card_id = 111,
      //         link_type = ""
      //     };
      //     object[] links_to_new_cards_to_add_Array = { links_to_existing_cards_to_add_or_updateObject };
      //     var subtasks_to_convert_into_cards_Object = new
      //     {
      //         subtask_id = subtaskId,
      //         lane_id = 1,
      //         column_id = 1,
      //         title = "CONVERT TO CARD",
      //         links_to_existing_cards_to_add_or_update = links_to_new_cards_to_add_Array
      //     };
      //     object[] subtasks_to_convert_into_cards = { subtasks_to_convert_into_cards_Object };
      //     var reqBody = new { subtasks_to_convert_into_cards };
      //     var reqBodyJson = JsonSerializer.Serialize(reqBody);
      //
      //     var request = new RestRequest(secondaryUrl + "/19", Method.Patch);
      //     request.AddParameter("application/json", reqBodyJson, ParameterType.RequestBody);
      //     request.AddHeader(username, password);
      //     var response = client.Execute(request);
      //     var responseBody = JsonSerializer.Deserialize<ErrorResponse<List<string>>>(response.Content);
      //
      //     Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
      //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
      //     Assert.That(responseBody.error.details.subtasks_to_convert_into_cards.items.zero.
      //         links_to_existing_cards_to_add_or_update.items.zero.link_type[0],
      //         Is.EqualTo("The value must be one of the following: parent, child, relative, predecessor, successor."));
      // }












        private Body CreateBody(int? lane_id, int? column_id, int? workflow_id, int? position, int? priority, string title, string color)
        {
            Body newBody = new Body
            {
                lane_id = lane_id,
                column_id = column_id,
                workflow_id = workflow_id,
                position = position,
                priority = priority,
                title = title,
                color = color

            };
            return newBody;
        }
        private Body CreateSubtaskBody( int? position, string description)
        {
            Body body = new Body
            {
                description = description,
                position = position,
               
            };
            return body;
        }

        private RestRequest CreateRequestPost(Body body, string secondaryUrl)
        {
            var request = new RestRequest(secondaryUrl, Method.Post);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            request.AddHeader(username, password);
            request.Timeout = 2000;

            return request;
        }
        private RestRequest CreateRequestGet(int? card)
        {
           var request = new RestRequest(secondaryUrl + $"/{card}", Method.Get);
            request.AddHeader(username, password);
            
            request.Timeout = 2000;

            return request;
        }

        private RestRequest CreateRequestPatch(Body body, int? card)
        {
            var request = new RestRequest(secondaryUrl + $"/{card}", Method.Patch);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            request.AddHeader(username, password);
            request.Timeout = 2000;
            return request;
        }

        private RestRequest CreatedCard()
        {
            Body newBody = CreateBody(1, 2, 1, 0, 100, "NewTestCard", "F8F32B");
            var request = CreateRequestPost(newBody, secondaryUrl); 
            return request;
        }
        private Body CreateSizeBody(int? lane_id,int? column_id, int? size,  string title)
        {
            Body newSizeBody = new Body
            {
                lane_id = lane_id,
                column_id = column_id,
                size = size,
                title = title, 

            };
            return newSizeBody;
        }

    }
}