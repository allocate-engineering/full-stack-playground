<template>
  <div class="modal-container">
    <div class="modal">
      <div class="close" @click="$emit('closeModal')"><div>+</div></div>
      <slot></slot>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Modal',
};
</script>

<style lang="scss" scoped>
.modal-container {
  position: fixed;
  top: 0;
  bottom: 0;
  right: 0;
  left: 0;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;

  .modal {
    width: 800px;
    background: #304552;
    border: 1px solid #497383;
    height: 600px;
    padding: 20px;
    box-sizing: border-box;
    position: relative;
    overflow-y: scroll;
    overflow-x: hidden;

    .close {
      position: absolute;
      right: 25px;
      top: 0;
      font-size: 44px;
      color: white;

      div {
        transform: rotate(45deg);
        cursor: pointer;
        transition: all 150ms ease-out;

        &:hover {
          transform: rotate(45deg) scale(1.3);
        }
      }
    }
  }

  // .veil{
  //   position: absolute;
  //   top:0
  // }
}
</style>
