<template>
    <div class="app-login">
        <div>email: <input id="email" type="text" label="Email" v-model="email" /></div>
        <div>password: <input id="password" type="text" label="Password" v-model="password" /></div>
        
        <div>username is: qesandbox@gmail.com</div>
        <div>password is: test123</div>
        <button id="sign-in-button-manual" @click="loginEmailPassword">login with email and password</button>
    </div>
</template>

<script>
    import {useMainStore} from "../store"
    export default {
        setup(){
            const mainStore = useMainStore();
            return {
                mainStore
            }
        },
        data() {
            return {
                email: "qesandbox@gmail.com",
                password: "",
            };
        },
        methods: {
            async loginEmailPassword() {
                try {
                    await this.mainStore.loginEmailPass({ email: this.email, password: this.password });
                } catch (ex) {
                    alert(ex.message);
                }
            },
        },
    }
</script>

<style lang="scss">
    .app-login {
        hr {
            margin: 20px 0 60px;
        }
        button {
            margin: 0 auto;
        }
    }
</style>