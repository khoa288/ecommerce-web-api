class LocalStorageService {
    setUsername(username) {
        localStorage.setItem("username", username);
    }
  
    getUsername() {
        return localStorage.getItem("username");
    }
  
    removeUsername() {
        localStorage.removeItem("username");
    }
}

export default LocalStorageService;