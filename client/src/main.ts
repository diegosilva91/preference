import Vue from 'vue';
import App from './App.vue';
import router from './router';
import BootstrapVue from 'bootstrap-vue';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-vue/dist/bootstrap-vue.css';
import QuestionHub from './issue-hub';

Vue.config.productionTip = false;
Vue.use(QuestionHub);
Vue.use(BootstrapVue);

new Vue({
  router,
  render: (h) => h(App),
}).$mount('#app');
