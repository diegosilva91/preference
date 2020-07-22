<template>
  <div class="hello">
    <h1>{{ msg }}</h1>
    <p>
      For a guide and recipes on how to configure / customize this project,
      <br />check out the
      <a
        href="https://cli.vuejs.org"
        target="_blank"
        rel="noopener"
      >vue-cli documentation</a>.
    </p>
    
      <section class="todoapp">
        
        <header class="header">
          <h1>todos</h1>
          <b-input-group prepend="Issue" class="mb-2">
          <b-form-input
            class="new-todo mr-4"
            autocomplete="off"
            v-model="newIssueTitle"
            requiered
          />
          <b-form-input 
            class="new-todo mr-4"
            v-model="newIssueDescription"
            autocomplete="off"
            requiered
            />
          <b-form-input 
            class="new-todo mr-4"
            v-model="newIssueSeverity"
            autocomplete="off"
            requiered
          />
          <b-form-input 
            class="new-todo mr-4"
            v-model="newIssueStatus"
            autocomplete="off"
            required
            @keyup.enter="addIssue"
          />
          <b-button variant="info" @click="addIssue">Button</b-button>
          </b-input-group>
        </header>
        
        <section class="main" v-if="todos" v-show="todos">
          <!--<input id="toggle-all" class="toggle-all" type="checkbox" v-model="allDone" />-->
          <label for="toggle-all"></label>
          <b-list-group >
            <b-list-group-item
             v-for="todo in filteredTodos"
             :key="todo.id"
                class="todo"
                >
            <!--<ul class="todo-list">
              <li
                :key="todo.title"
                :class="{ completed: todo.completed, editing: todo == editedTodo }"
              >
                <div class="view">-->
                <!--  <input class="toggle" type="checkbox" v-model="todo.completed" />
                  <label @dblclick="editTodo(todo)">{{ todo.title }}</label>-->
                  <!--<button class="destroy" @click="removeTodo(todo)"></button>
                  v-todo-focus="todo == editedTodo"
                  @blur="doneEdit(todo)"
                </div>-->
                <input
                  class="edit"
                  type="text"
                  v-model="todo.title"
                  
                  @keyup.enter="doneEdit(todo)"
                  @keyup.esc="cancelEdit(todo)"
                />
              <!--</li>
            </ul>-->
            <div class="d-flex w-100 justify-content-between">
              <div class="issue-severity">
                <!--<h5  :key="todo.severity" class="mb-1" @dblclick="editTodo(todo)">{{todo.severity}}-->
                <input
                    class="edit"
                    type="text"
                    v-model="todo.severity"
                    
                  
                  @keyup.enter="doneEdit(todo)"
                  @keyup.esc="cancelEdit(todo)"             
                  />                
                  
                <!--</h5>-->
              </div>
              <small>{{todo.internalId.creationTime}}</small>
            </div>
            <p class="mb-1">
              <input
                    class=""
                    type="text"
                    v-model="todo.description"
    
                  @keyup.enter="doneEdit(todo)"
                  @keyup.esc="cancelEdit(todo)"                   
                  />                
              <!--{{todo.description}}-->
            </p>
            <small>
              <input
                    class=""
                    type="text"
                    v-model="todo.status"
    
                  @keyup.enter="doneEdit(todo)"
                  @keyup.esc="cancelEdit(todo)"              
                  />                
              <!--{{todo.status}}-->
            </small>
            </b-list-group-item>
          </b-list-group>
        </section>
      </section>
      <footer class="info">
        <p>Double-click to edit a todo</p>
      </footer>
    
  </div>
</template>

<script lang="ts">
import Vue from 'vue';

import { getElements,addElements,updateElement } from '../axios-service.js';
const STORAGE_KEY = 'issue-vuejs-2.0';
const todoStorage = {
  fetch: () => {
    let todos ='';
     //JSON.parse(localStorage.getItem(STORAGE_KEY) || '[]');
    return todos;
  },
  save: (todos: any) => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(todos));
  },
};
const IssueApi={
  save:(todos)=>{
    if(todos)
    todos.forEach((todo: any) => {
      updateElement('api/todos',todo.id,todo, (error, data)=> {
        if(error)
          console.log(error);
        else
          console.log("Element saved: ", data);
      });
    });
  },
  load:()=>{
    let response= getElements('api/todos',(error, data)=> {
      if(error){
        console.log(error);
      }
      else{
        console.log("data load from API",data);
        todoStorage.save(data);
        return data;
      }
    });
    return response;
  },
};

// visibility filters
var filters = {
  all: function(todos:any) {
    return todos;
  },
  active: function(todos:any) {
    return todos.filter(function(todo:any) {
      return !todo.completed;
    });
  },
  completed: function(todos:any) {
    return todos.filter(function(todo:any) {
      return todo.completed;
    });
  }
};

export default Vue.extend({
  name: "HelloWorld",
  props: {
    msg: String
  },

  data: () => {
    return {
      todos:'',
      newIssueTitle: '',
      newIssueDescription:'',
      newIssueSeverity:'',
      newIssueStatus:'',
      editedTodo: null,
      visibility: "all",
      connection:null,
      issue:{
        type: Object,
        required: true
      }
      
    };
  },
  
  watch: {
    IssueApi: {
        handler:function (){
          this.$questionHub.$on('issue-changed', this.onIssueChanged);
        },
        deep:true 
      },
    todos: {
      handler: function(todos) {
        todoStorage.save(todos);
        IssueApi.save(todos);
      },
      deep: true
    }
  },
  // computed properties
  // http://vuejs.org/guide/computed.html
  computed: {
    filteredTodos: function() {
      return filters[this.visibility](this.todos);
    },
    remaining: function() {
      return filters.active(this.todos).length;
    },
    allDone: {
      get: function() {
        return this.remaining === 0;
      },
      set: function(value) {
        this.todos.forEach(function(todo) {
          todo.completed = value;
        });
      }
    }
  },
  filters: {
    pluralize: function(n) {
      return n === 1 ? "item" : "items";
    }
  },
  created:function(){
    this.todos=IssueApi.load();
    this.$questionHub.$on('issue-changed', this.onIssueChanged);

  },
  mounted:function(){
    this.todos=IssueApi.load();
    this.$questionHub.$on('issue-changed', this.onIssueChanged);
  },
  
  // methods that implement data logic.
  // note there's no DOM manipulation here at all.
  methods: {

    onIssueChanged (issue) {
      console.log("data changed from server",issue.Issue);
      this.todos=issue.Issue;
    },
    addIssue: async function() {
      let  vm=this;
      var title = this.newIssueTitle && this.newIssueTitle.trim();
      if (!title) {
        return;
      }
      var description = this.newIssueDescription && this.newIssueDescription.trim();
      if (!description) {
        return;
      }

      var severity = this.newIssueSeverity && this.newIssueSeverity.trim();
      if (!severity) {
        return;
      }

      var status = this.newIssueStatus && this.newIssueStatus.trim();
      if (!status) {
        return;
      }
      let issue={
        title: title,
        description:description,
        severity:severity,
        status:status
      };
      /*this.todos.push({
        //id: todoStorage.uid++,
        
        //completed: false
      });*/
      this.newIssueTitle = "";
      this.newIssueDescription="";
      this.newIssueSeverity="";
      this.newIssueStatus="";
      addElements('api/todos',issue,(error,data)=>{
      if(error){
        console.log(error);
      }
      else{
        console.log("issue save to API",data);
        vm.todos.push(data);
      }
      });
    },

    removeTodo: function(todo:any) {
      this.todos.splice(this.todos.indexOf(todo), 1);
    },

    editTodo: function(todo:any) {
      this.beforeEditCache = todo;
      this.editedTodo = todo;
    },

    doneEdit: function(todo) {
      if (!this.editedTodo) {
        return;
      }
      this.editedTodo = null;
      //todo= todo.trim();
      if (!todo) {
        this.removeTodo(todo);
      }
    },

    cancelEdit: function(todo) {
      this.editedTodo = null;
      todo = this.beforeEditCache;
    },

    removeCompleted: function() {
      this.todos = filters.active(this.todos);
    }
  },
  // a custom directive to wait for the DOM to be updated
  // before focusing on the input field.
  // http://vuejs.org/guide/custom-directive.html
  directives: {
    'todo-focus': function(el, binding) {
      if (binding.value) {
        el.focus();
      }
    }
  }
});
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

html,
body {
  margin: 0;
  padding: 0;
}

button {
  margin: 0;
  padding: 0;
  border: 0;
  background: none;
  font-size: 100%;
  vertical-align: baseline;
  font-family: inherit;
  font-weight: inherit;
  color: inherit;
  -webkit-appearance: none;
  appearance: none;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

body {
  font: 14px "Helvetica Neue", Helvetica, Arial, sans-serif;
  line-height: 1.4em;
  background: #f5f5f5;
  color: #4d4d4d;
  min-width: 230px;
  max-width: 550px;
  margin: 0 auto;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  font-weight: 300;
}

:focus {
  outline: 0;
}

.hidden {
  display: none;
}

.todoapp {
  background: #fff;
  margin: 130px 0 40px 0;
  position: relative;
  box-shadow: 0 2px 4px 0 rgba(0, 0, 0, 0.2), 0 25px 50px 0 rgba(0, 0, 0, 0.1);
}

.todoapp input::-webkit-input-placeholder {
  font-style: italic;
  font-weight: 300;
  color: #e6e6e6;
}

.todoapp input::-moz-placeholder {
  font-style: italic;
  font-weight: 300;
  color: #e6e6e6;
}

.todoapp input::input-placeholder {
  font-style: italic;
  font-weight: 300;
  color: #e6e6e6;
}

.todoapp h1 {
  position: absolute;
  top: -190px;
  width: 100%;
  font-size: 100px;
  font-weight: 100;
  text-align: center;
  color: rgba(175, 47, 47, 0.15);
  -webkit-text-rendering: optimizeLegibility;
  -moz-text-rendering: optimizeLegibility;
  text-rendering: optimizeLegibility;
}

.new-todo,
.edit {
  position: relative;
  margin: 0;
  width: 100%;
  font-size: 24px;
  font-family: inherit;
  font-weight: inherit;
  line-height: 1.4em;
  border: 0;
  color: inherit;
  padding: 6px;
  border: 1px solid #999;
  box-shadow: inset 0 -1px 5px 0 rgba(0, 0, 0, 0.2);
  box-sizing: border-box;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

.new-todo {
  padding: 16px 16px 16px 60px;
  border: none;
  background: rgba(0, 0, 0, 0.003);
  box-shadow: inset 0 -2px 1px rgba(0, 0, 0, 0.03);
}

.main {
  position: relative;
  z-index: 2;
  border-top: 1px solid #e6e6e6;
}

.toggle-all {
  width: 1px;
  height: 1px;
  border: none; /* Mobile Safari */
  opacity: 0;
  position: absolute;
  right: 100%;
  bottom: 100%;
}

.toggle-all + label {
  width: 60px;
  height: 34px;
  font-size: 0;
  position: absolute;
  top: -52px;
  left: -13px;
  -webkit-transform: rotate(90deg);
  transform: rotate(90deg);
}

.toggle-all + label:before {
  content: "❯";
  font-size: 22px;
  color: #e6e6e6;
  padding: 10px 27px 10px 27px;
}

.toggle-all:checked + label:before {
  color: #737373;
}

.todo-list {
  margin: 0;
  padding: 0;
  list-style: none;
}

.todo-list li {
  position: relative;
  font-size: 24px;
  border-bottom: 1px solid #ededed;
}

.todo-list li:last-child {
  border-bottom: none;
}

.todo-list li.editing {
  border-bottom: none;
  padding: 0;
}

.todo-list li.editing .edit {
  display: block;
  width: calc(100% - 43px);
  padding: 12px 16px;
  margin: 0 0 0 43px;
}

.todo-list li.editing .view {
  display: none;
}

.todo-list li .toggle {
  text-align: center;
  width: 40px;
  /* auto, since non-WebKit browsers doesn't support input styling */
  height: auto;
  position: absolute;
  top: 0;
  bottom: 0;
  margin: auto 0;
  border: none; /* Mobile Safari */
  -webkit-appearance: none;
  appearance: none;
}

.todo-list li .toggle {
  opacity: 0;
}

.todo-list li .toggle + label {
  /*
		Firefox requires `#` to be escaped - https://bugzilla.mozilla.org/show_bug.cgi?id=922433
		IE and Edge requires *everything* to be escaped to render, so we do that instead of just the `#` - https://developer.microsoft.com/en-us/microsoft-edge/platform/issues/7157459/
	*/
  background-image: url("data:image/svg+xml;utf8,%3Csvg%20xmlns%3D%22http%3A//www.w3.org/2000/svg%22%20width%3D%2240%22%20height%3D%2240%22%20viewBox%3D%22-10%20-18%20100%20135%22%3E%3Ccircle%20cx%3D%2250%22%20cy%3D%2250%22%20r%3D%2250%22%20fill%3D%22none%22%20stroke%3D%22%23ededed%22%20stroke-width%3D%223%22/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: center left;
}

.todo-list li .toggle:checked + label {
  background-image: url("data:image/svg+xml;utf8,%3Csvg%20xmlns%3D%22http%3A//www.w3.org/2000/svg%22%20width%3D%2240%22%20height%3D%2240%22%20viewBox%3D%22-10%20-18%20100%20135%22%3E%3Ccircle%20cx%3D%2250%22%20cy%3D%2250%22%20r%3D%2250%22%20fill%3D%22none%22%20stroke%3D%22%23bddad5%22%20stroke-width%3D%223%22/%3E%3Cpath%20fill%3D%22%235dc2af%22%20d%3D%22M72%2025L42%2071%2027%2056l-4%204%2020%2020%2034-52z%22/%3E%3C/svg%3E");
}

.todo-list li label {
  word-break: break-all;
  padding: 15px 15px 15px 60px;
  display: block;
  line-height: 1.2;
  transition: color 0.4s;
}

.todo-list li.completed label {
  color: #d9d9d9;
  text-decoration: line-through;
}

.todo-list li .destroy {
  display: none;
  position: absolute;
  top: 0;
  right: 10px;
  bottom: 0;
  width: 40px;
  height: 40px;
  margin: auto 0;
  font-size: 30px;
  color: #cc9a9a;
  margin-bottom: 11px;
  transition: color 0.2s ease-out;
}

.todo-list li .destroy:hover {
  color: #af5b5e;
}

.todo-list li .destroy:after {
  content: "×";
}

.todo-list li:hover .destroy {
  display: block;
}

.todo-list li .edit {
  display: none;
}

.todo-list li.editing:last-child {
  margin-bottom: -1px;
}

.footer {
  color: #777;
  padding: 10px 15px;
  height: 20px;
  text-align: center;
  border-top: 1px solid #e6e6e6;
}

.footer:before {
  content: "";
  position: absolute;
  right: 0;
  bottom: 0;
  left: 0;
  height: 50px;
  overflow: hidden;
  box-shadow: 0 1px 1px rgba(0, 0, 0, 0.2), 0 8px 0 -3px #f6f6f6,
    0 9px 1px -3px rgba(0, 0, 0, 0.2), 0 16px 0 -6px #f6f6f6,
    0 17px 2px -6px rgba(0, 0, 0, 0.2);
}

.todo-count {
  float: left;
  text-align: left;
}
.todo-count strong {
  font-weight: 300;
}

.filters {
  margin: 0;
  padding: 0;
  list-style: none;
  position: absolute;
  right: 0;
  left: 0;
}

.filters li {
  display: inline;
}

.filters li a {
  color: inherit;
  margin: 3px;
  padding: 3px 7px;
  text-decoration: none;
  border: 1px solid transparent;
  border-radius: 3px;
}

.filters li a:hover {
  border-color: rgba(175, 47, 47, 0.1);
}

.filters li a.selected {
  border-color: rgba(175, 47, 47, 0.2);
}

.clear-completed,
html .clear-completed:active {
  float: right;
  position: relative;
  line-height: 20px;
  text-decoration: none;
  cursor: pointer;
}

.clear-completed:hover {
  text-decoration: underline;
}

.info {
  margin: 65px auto 0;
  color: #bfbfbf;
  font-size: 10px;
  text-shadow: 0 1px 0 rgba(255, 255, 255, 0.5);
  text-align: center;
}

.info p {
  line-height: 1;
}

.info a {
  color: inherit;
  text-decoration: none;
  font-weight: 400;
}

.info a:hover {
  text-decoration: underline;
}

/*
	Hack to remove background from Mobile Safari.
	Can't use it globally since it destroys checkboxes in Firefox
*/
@media screen and (-webkit-min-device-pixel-ratio: 0) {
  .toggle-all,
  .todo-list li .toggle {
    background: none;
  }

  .todo-list li .toggle {
    height: 40px;
  }
}

@media (max-width: 430px) {
  .footer {
    height: 50px;
  }

  .filters {
    bottom: 10px;
  }
}
</style>


    