import axios from "axios";

export default axios.create({
  baseURL: "https://localhost:7198/api", // your api port
  headers: {
    "X-Lab-Id":
      "C44C3245-7818-4A77-8DE3-4E11DD326CFA"
  }
});