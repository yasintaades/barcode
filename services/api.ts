const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7001/api';

export const printService = {
  startSftpStream: async () => {
    const response = await fetch(`${API_BASE_URL}/print/start-sftp-stream`, {
      method: 'POST',
    });
    return response.json();
  },
};