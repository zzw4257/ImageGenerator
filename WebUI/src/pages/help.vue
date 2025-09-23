<template>
  <v-container class="py-6">
    <v-card rounded="xl" elevation="2" class="mb-6">
      <v-card-title class="pa-6 d-flex align-center">
        <v-icon class="mr-3">mdi-help-circle</v-icon>
        Help & FAQ
      </v-card-title>
      <v-divider />
      <v-card-text class="pa-6">
        <h3 class="text-subtitle-1 font-weight-bold mb-4">使用条款与内容政策</h3>
        <v-alert type="warning" variant="tonal" class="mb-4" density="comfortable" :border="'start'" icon="mdi-alert">
          为保障平台健康运行与所有用户的合法权益，请严格遵守以下规则：
        </v-alert>
        <ul class="text-body-2 mb-6 ml-6">
          <li>禁止生成或传播违法内容，包括但不限于：色情、暴力、血腥、仇恨、恐怖主义、骚扰、歧视、煽动犯罪等。</li>
          <li>禁止侵犯他人合法权益，包括但不限于版权、商标、肖像权、隐私权与商业秘密。</li>
          <li>平台可能对内容进行自动或人工审核；一经发现违规，账号可能被限制功能、清空数据或封禁。</li>
          <li>开发者保留对产品功能、服务变更、账号管理、数据清理、封禁与仲裁的最终决定权。</li>
          <li>平台不对用户生成内容的合法性承担责任，使用者需自行确保合规并承担相应风险与义务。</li>
        </ul>

        <h3 class="text-subtitle-1 font-weight-bold mb-2">邀请（Invitation）机制</h3>
        <ul class="text-body-2 mb-6 ml-6">
          <li>注册需要有效的邀请码。邀请码可由已授权用户或管理员在「Invitation Codes」页面创建并分享。</li>
          <li>每个邀请码具有使用次数/有效期等限制（以实际配置为准）。在注册页输入邀请码即可完成注册。</li>
          <li>滥用或倒卖邀请码的行为将被封禁处理，相关账户与使用记录可能被清理。</li>
        </ul>

        <v-alert type="info" variant="tonal" class="mb-4" density="comfortable" :border="'start'" icon="mdi-clock">
          邀请模式可能会根据平台策略调整，请关注官方公告获取最新信息。
        </v-alert>

        <h3 class="text-subtitle-1 font-weight-bold mb-2">积分（Credits）机制</h3>
        <ul class="text-body-2 mb-6 ml-6">
          <li>每日可在「Profile」页面通过「Claim Daily Credits」按钮领取日常积分（额度与频次以实际配置为准）。</li>
          <li>生成图片会消耗一定数量的积分（消耗标准可能随模型与质量设定调整）。</li>
          <li>积分不得转让、套现或倒卖；若存在违规使用，平台有权清零积分并限制功能。</li>
        </ul>

        <h3 class="text-subtitle-1 font-weight-bold mb-2">快捷方式</h3>
        <v-table density="comfortable" class="mb-6">
          <thead>
            <tr>
              <th>操作</th>
              <th>快捷键</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>生成图片</td>
              <td>Shift + Enter</td>
            </tr>
          </tbody>
        </v-table>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<style scoped>
ul li{
  margin-bottom: 5px;
}
</style>

<route lang="yaml">
meta:
  title: Help
</route>
