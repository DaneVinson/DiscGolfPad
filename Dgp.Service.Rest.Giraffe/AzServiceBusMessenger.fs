namespace Dgp.Service.Rest.Giraffe

open System
open System.Text
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Azure.ServiceBus
open FSharp.Control.Tasks.V2
open Newtonsoft.Json
open Giraffe
open Dgp.Domain.Core

module AzServiceBusMessenger =

    let private sendMessageAsync =
        fun msg (messageType:string) (connection:string) (queue:string) (playerId:string) ->
            task {
                let playerMessage = { 
                    PlayerId = playerId; 
                    Type = messageType; 
                    Data = ServiceUtility.serializeToJson msg;
                }
                let message = new Message(Encoding.UTF8.GetBytes(ServiceUtility.serializeToJson playerMessage))
                message.ContentType <- msg.GetType().ToString()
                return! (new QueueClient(connection, queue)).SendAsync message
            }

    let sendCommandMessageAsync (context:HttpContext) = 
        fun command (commandType:string) ->
                let connection = context.GetService<IConfiguration>()
                                        .GetValue("AzureServiceBusOptions:ConnectionString")
                let playerId = ServiceUtility.getPlayerId context
                sendMessageAsync command commandType connection "commands" playerId
