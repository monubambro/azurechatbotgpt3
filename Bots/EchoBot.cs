﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        private HttpClient _httpClient;


        
        
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //var replyText = $"Echo: {turnContext.Activity.Text}";

            var prompt = turnContext.Activity.Text;
            var conversationHistory = string.Empty;
            
            var openaiResponse = await PostDataToEndpoint(prompt);

            conversationHistory += $"User: {turnContext.Activity.Text}\nChatbot: {openaiResponse.choices[0].text}\n";
            var replyText = $"{openaiResponse.choices[0].text} [~{openaiResponse.usage.total_tokens} tokens]";
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);

                       
        }

        private async Task<dynamic> PostDataToEndpoint(string prompt1)
        {
                      

            var json = JsonConvert.SerializeObject(new
            {
                prompt = prompt1,
                temperature = 0.5,
                max_tokens = 50,
                model = "text-davinci-003"
            });

            _httpClient = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            //TODO PUT YOUR API KEY
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + "");
            var response = await _httpClient.PostAsync(" https://api.openai.com/v1/completions", content);


            if (!response.IsSuccessStatusCode)
            {
                // Handle error
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<dynamic>(responseJson);

            //return result.choices[0].text;

        }


        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
