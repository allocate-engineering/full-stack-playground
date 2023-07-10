import { createWebHistory, createRouter } from "vue-router";
import Home from "@/pages/Home.vue";
import Success from "@/pages/Success.vue";
import NotFound from "@/pages/NotFound.vue";

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home,
  },
  {
    path: "/success",
    name: "Success",
    component: Success,
  },
  {
    path: "/:pathMatch(.*)*",
    name: "NotFound",
    component: NotFound,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
