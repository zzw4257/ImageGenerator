import axios from "axios";

const instance = axios.create({
  baseURL: "/api",
});

instance.interceptors.request.use(
  (config) => {
    let token = localStorage.getItem("token");
    if (token && config.headers)
        config.headers['Authorization'] = `Bearer ${localStorage.getItem('token')}`;
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

declare module "vue" {
  interface ComponentCustomProperties {
    $axios: typeof instance;
  }
}

export default instance;
