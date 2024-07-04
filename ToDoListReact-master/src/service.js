import axios from 'axios';
import config from './config';
const apiUrl = "https://todolistserverayalaku.onrender.com";
console.log('API URL:', apiUrl);

const responseInterceptor = axios.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    console.log(error.message)
    return Promise.reject(error); }
);

export default {
  getTasks: async () => {
    const result = await axios.get(`https://todolistserverayalaku.onrender.com/items`)    
    return result.data;
  },

  addTask: async (name) => {

    try {
      const response = await axios.post(`${apiUrl}/items`, {
        Name: name,
        IsComplete: false, // Assuming you want to set IsComplete to false for new tasks
      });

      return response.data;
    } catch (error) {
      // Handle error
      return null;
    }
  },
  setCompleted: async (id, isComplete) => {

    try {
      const response = await axios.put(`${apiUrl}/items/${id}`, {
        IsComplete: isComplete,
      });

      return response.data;
    } catch (error) {
      // Handle error
      return null;
    }
  },

  deleteTask: async (id) => {

    try {
      const response = await axios.delete(`${apiUrl}/items/${id}`);

      if (response.status === 200) {
        console.log('Task deleted successfully');
      } else {
        // Handle unexpected response status
        console.error('Error deleting task:', response.statusText);
      }
    } catch (error) {
      // Handle error
    }
  },
};
