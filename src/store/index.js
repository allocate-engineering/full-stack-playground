import firebase from "firebase";
import {defineStore} from "pinia";

export const useMainStore = defineStore('main', {
  state: ()=> ({
    isDevelopment: process.env.NODE_ENV !== "production",
    currentUser: null,
    globalLoading: true,
    firebase: null,
    layout: "desktop"
  }),
  getters: {
    isMobile: (state) => state.layout === "mobile",
  },
  actions: {
    updateLayout(){
      let currentLayout = getCurrentLayout();

      if (currentLayout !== this.layout) {
        this.layout = layout;
      }
    },
    async loginEmailPass(val){
      let userCredential = await firebase.auth().signInWithEmailAndPassword(val.email, val.password);
      this.currentUser = userCredential.user;
    },
    async logout(){
      await firebase.auth().signOut();
      this.currentUser = null;
    }
  }
})
