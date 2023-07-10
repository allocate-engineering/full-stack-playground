<template>
  <div class="box-container">
    <div class="title">
      <img v-if="icon === 'cluster-icon'" src="@/assets/cluster-icon.svg" />
      <img v-if="icon === 'container-icon'" src="@/assets/container-icon.svg" />
      <img v-if="icon === 'pipe-icon'" src="@/assets/pipe-icon.svg" />
      <img v-if="icon === 'money-icon'" src="@/assets/money-icon.svg" />
      {{ title }}
    </div>
    <div class="box-content">
      <slot />
    </div>
  </div>
</template>

<script>
export default {
  name: 'BoxContainer',
  props: {
    title: {
      name: 'title',
      required: true,
    },
    icon: {
      name: 'icon',
      required: false,
      default: 'pipe-icon.svg',
    },
  },
  computed: {
    parseIcon: () => {},
  },
};
</script>

<style lang="scss" scoped>
.title {
  font-family: Roboto;
  font-style: normal;
  font-weight: normal;
  font-size: 14px;
  line-height: 16px;
  margin-top: 42px;
  margin-bottom: 8px;

  img {
    margin-right: 7px;
    max-height: 20px;
  }
}

.grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
}

.box-content {
  background: #202a37;
  padding: 30px 40px;
  width: 100%;
  box-sizing: border-box;
  max-width: 100%;
}

@include responsive(mobile) {
  .box-content {
    padding: 30px 16px;
  }
  .title {
    padding-left: 7px;
  }
}
</style>
