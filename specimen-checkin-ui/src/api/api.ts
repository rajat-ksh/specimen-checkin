import axios from "axios";

export default axios.create({
  baseURL: "https://localhost:7198/api", // your api port
  headers: {
    "X-Lab-Id":
      "11111111-1111-1111-1111-111111111111"
  }
});