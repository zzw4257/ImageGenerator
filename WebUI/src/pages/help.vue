<template>
  <v-container class="py-6">
    <v-card rounded="xl" elevation="2" class="mb-6">
      <v-card-title class="pa-6 d-flex align-center">
        <v-icon class="mr-3">mdi-help-circle</v-icon>
        Help & FAQ
      </v-card-title>
      <v-divider />
      <v-card-text class="pa-6">
        <h3 class="text-subtitle-1 font-weight-bold mb-4">Terms of Use and Content Policy</h3>
        <v-alert type="warning" variant="tonal" class="mb-4" density="comfortable" :border="'start'" icon="mdi-alert">
          To ensure the healthy operation of the platform and the legitimate rights of all users, please strictly adhere to the following rules:
        </v-alert>
        <ul class="text-body-2 mb-6 ml-6">
          <li>It is forbidden to generate or disseminate illegal content, including but not limited to: pornography, violence, gore, hatred, terrorism, harassment, discrimination, incitement to crime, etc.</li>
          <li>It is forbidden to infringe on the legitimate rights and interests of others, including but not limited to copyright, trademark, portrait rights, privacy rights, and trade secrets.</li>
          <li>The platform may conduct automatic or manual content review; upon discovery of violations, the account may be restricted, have its data cleared, or be banned.</li>
          <li>The developer reserves the final right to decide on product features, service changes, account management, data clearing, bans, and arbitration.</li>
          <li>The platform is not responsible for the legality of user-generated content; users must ensure compliance and assume corresponding risks and obligations.</li>
        </ul>

        <h3 class="text-subtitle-1 font-weight-bold mb-2">Invitation Mechanism</h3>
        <ul class="text-body-2 mb-6 ml-6">
          <li>A valid invitation code is required for registration. Invitation codes can be created and shared by authorized users or administrators on the "Invitation Codes" page.</li>
          <li>Each invitation code has usage limits/validity periods (subject to actual configuration). Registration can be completed by entering the invitation code on the registration page.</li>
          <li>Abuse or resale of invitation codes will result in a ban, and related accounts and usage records may be cleared.</li>
        </ul>

        <v-alert type="info" variant="tonal" class="mb-4" density="comfortable" :border="'start'" icon="mdi-clock">
          The invitation model may be adjusted according to platform strategy. Please pay attention to official announcements for the latest information.
        </v-alert>

        <h3 class="text-subtitle-1 font-weight-bold mb-2">Credits Mechanism</h3>
        <ul class="text-body-2 mb-6 ml-6">
          <li>Daily credits can be claimed on the "Profile" page via the "Claim Daily Credits" button (amount and frequency are subject to actual configuration).</li>
          <li>Generating images consumes a certain number of credits (the consumption standard may be adjusted with model and quality settings).</li>
          <li>Credits cannot be transferred, cashed out, or resold; in case of improper use, the platform has the right to clear credits and restrict functions.</li>
        </ul>

        <h3 class="text-subtitle-1 font-weight-bold mb-2">Shortcuts</h3>
        <v-table density="comfortable" class="mb-6">
          <thead>
            <tr>
              <th>Action</th>
              <th>Shortcut</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Generate Image</td>
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
