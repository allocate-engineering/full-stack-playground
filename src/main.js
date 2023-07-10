import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import { createPinia } from 'pinia'


let app = createApp(App);
const pinia = createPinia();
app.use(router).use(pinia);

app.mount("#app");
