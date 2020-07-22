import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'

export default {
  install (Vue) {
    const questionHub = new Vue() 
    // every component will use this.$questionHub to access the event bus
    Vue.prototype.$questionHub = questionHub
    // Forward server side SignalR events through $questionHub, where components will listen to them
    
    const connection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/issue-hub')
      .configureLogging(LogLevel.Information)
      .build()


    //connection.start()
    connection.onclose(() => {
      connectToSignalR();
    })
    connectToSignalR();
    connection.on('IssuesChanges', (Issue) => {
      questionHub.$emit('issue-changed', { Issue })
      console.log("conected to server");
    })
    
    function connectToSignalR() {
      connection.start().catch(err => {
          console.error('Failed to connect with hub', err)
          return new Promise((resolve, reject) =>
              setTimeout(() => connectToSignalR().then(resolve).catch(reject), 1000))
      })
    }
  }
}
