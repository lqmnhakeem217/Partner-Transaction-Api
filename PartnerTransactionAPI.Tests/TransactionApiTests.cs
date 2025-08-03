using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PartnerTransactionAPI.DTOs;
using PartnerTransactionAPI.Utils;
using PartnerTransactionAPI.Constants;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PartnerTransactionAPI.Tests
{
    public class TransactionApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public TransactionApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostTransaction_ShouldReturnSuccess_WhenValidData()
        {
            var client = new WebApplicationFactory<Program>().CreateClient();

            var request = new TransactionDto
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes("FAKEPASSWORD1234")),
                TotalAmount = 1000,
                Timestamp = DateTime.UtcNow.ToString("o"),
                Items = new List<ItemDetailDTO>()
                {
                    new(){ PartnerItemRef = "i-00001",Name = "Pen", Quantity=4, UnitPrice=200 },
                    new(){ PartnerItemRef = "i-00002",Name = "Ruler", Quantity=2, UnitPrice=100 },
                }
            };

            request.Signature = SignatureUtil.GenerateSignature(request);

            var response = await client.PostAsJsonAsync("/api/submittrxmessage", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<ReceiptDto>(); ;
            content.Should().NotBeNull();
            content!.Result.Should().Be(1);
            content.FinalAmount.Should().Be(1000);
        }

        [Fact]
        public async Task PostTransaction_ShouldReturnFailed_WhenMissingPartnerId()
        {

            var client = new WebApplicationFactory<Program>().CreateClient();
            var request = new TransactionDto
            {
                PartnerKey = "",
                PartnerRefNo = "",
                PartnerPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes("password")),
                TotalAmount = 0,
                Timestamp = DateTime.UtcNow.ToString("o"),
                Signature = "dummy"
            };


            var response = await client.PostAsJsonAsync("/api/submittrxmessage", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadFromJsonAsync<ReceiptDto>();
            content!.Result.Should().Be(0);
            content.ResultMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task PostTransaction_WhenApiExpired()
        {
            var client = new WebApplicationFactory<Program>().CreateClient();

            var request = new TransactionDto
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes("FAKEPASSWORD1234")),
                TotalAmount = 1000,
                Timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)).ToString("o"),
                Items = new List<ItemDetailDTO>()
                {
                    new(){ PartnerItemRef = "i-00001",Name = "Pen", Quantity=4, UnitPrice=200 },
                    new(){ PartnerItemRef = "i-00002",Name = "Ruler", Quantity=2, UnitPrice=100 },
                }
            };

            request.Signature = SignatureUtil.GenerateSignature(request);

            var response = await client.PostAsJsonAsync("/api/submittrxmessage", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadFromJsonAsync<ReceiptDto>(); ;
            content.Should().NotBeNull();
            content!.Result.Should().Be(0);
            content.ResultMessage.Should().Be(ApiConstant.ErrorMessage.TimeStampExpired);
        }

        [Fact]
        public async Task PostTransaction_WhenPKeyNotAllowMisMatchSig()
        {
            var client = new WebApplicationFactory<Program>().CreateClient();

            var request = new TransactionDto
            {
                PartnerKey = "FAKEUSER",
                PartnerRefNo = "FG-00001",
                PartnerPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes("FAKEPASSWORD1234")),
                TotalAmount = 1000,
                Timestamp = DateTime.UtcNow.ToString("o"),
                Items = new List<ItemDetailDTO>()
                {
                    new(){ PartnerItemRef = "i-00001",Name = "Pen", Quantity=4, UnitPrice=200 },
                    new(){ PartnerItemRef = "i-00002",Name = "Ruler", Quantity=2, UnitPrice=100 },
                }
            };

            request.Signature = "badSignature";

            var response = await client.PostAsJsonAsync("/api/submittrxmessage", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadFromJsonAsync<ReceiptDto>(); ;
            content.Should().NotBeNull();
            content!.Result.Should().Be(0);
            content.ResultMessage.Should().Be(ApiConstant.ErrorMessage.AccessDenied);
        }

        [Fact]
        public async Task PostTransaction_WhenNoCondDisc()
        {
            var client = new WebApplicationFactory<Program>().CreateClient();

            var request = new TransactionDto
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes("FAKEPASSWORD1234")),
                TotalAmount = 100000,
                Timestamp = DateTime.UtcNow.ToString("o"),
                Items = new List<ItemDetailDTO>()
                {
                    new(){ PartnerItemRef = "i-00001",Name = "Pen", Quantity=5, UnitPrice=10000 },
                    new(){ PartnerItemRef = "i-00002",Name = "Ruler", Quantity=5, UnitPrice=10000 },
                }
            };

            request.Signature = SignatureUtil.GenerateSignature(request);

            var response = await client.PostAsJsonAsync("/api/submittrxmessage", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<ReceiptDto>(); ;
            content.Should().NotBeNull();
            content!.Result.Should().Be(1);
            content.TotalDiscount.Should().Be(10000);
        }

        [Fact]
        public async Task PostTransaction_WhenHasCondDisc()
        {
            var client = new WebApplicationFactory<Program>().CreateClient();

            var request = new TransactionDto
            {
                PartnerKey = "FAKEPEOPLE",
                PartnerRefNo = "FG-00002",
                PartnerPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes("FAKEPASSWORD4578")),
                TotalAmount = 120500,
                Timestamp = DateTime.UtcNow.ToString("o"),
                Items = new List<ItemDetailDTO>()
                {
                    new(){ PartnerItemRef = "i-00003",Name = "Eraser", Quantity=1, UnitPrice=500 },
                    new(){ PartnerItemRef = "i-00004",Name = "Car", Quantity=1, UnitPrice=100000 },
                    new(){ PartnerItemRef = "i-00005", Name = "T-Shirt", Quantity = 5, UnitPrice=4000 },

                }
            };

            request.Signature = SignatureUtil.GenerateSignature(request);

            var response = await client.PostAsJsonAsync("/api/submittrxmessage", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<ReceiptDto>(); ;
            content.Should().NotBeNull();
            content!.Result.Should().Be(1);
            content.TotalDiscount.Should().Be(24100);
            content.FinalAmount.Should().Be(96400);
        }
    }
}
