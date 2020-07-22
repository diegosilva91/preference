using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using preference.Models;
namespace server.Hubs
{
     /*public interface IIssueHub
     {
          Task IssuesChanges(, int score);
     }*/
     public class IssueHub: Hub
     {
         public async Task SendMyEvent()
        {
          await this.Clients.All.SendAsync("MyEvent");
        }
     }
}