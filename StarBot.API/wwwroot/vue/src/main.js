import { createApp } from 'vue';
import App from './App.vue';
import VueScrollTo from 'vue-scrollto';

const app = createApp(App);
app.use(VueScrollTo);
app.mount('#app');
