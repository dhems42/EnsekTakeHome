import React, { useState } from 'react';

const UploadCSV = () => {
  const [file, setFile] = useState(null);
  const [uploadStatus, setUploadStatus] = useState('');

  // Handle file selection
  const handleFileChange = (e:any) => {
    const selectedFile = e.target.files[0];
    if (selectedFile && selectedFile.type === 'text/csv') {
      setFile(selectedFile);
    } else {
      setUploadStatus('Please select a valid CSV file.');
    }
  };

  // Handle file upload to the server
  const handleFileUpload = async () => {
    if (!file) {
      setUploadStatus('No file selected');
      return;
    }

    const formData = new FormData();
    formData.append('file', file); // Append the file to FormData

    try {
      setUploadStatus('Uploading...');

      const response = await fetch('/api/meter-reading-uploads', {
        method: 'POST',
        body: formData, // Send FormData containing the file
      });

      if (response.ok) {
        const result = await response.json();
        setUploadStatus(result.data);
      } else {
        setUploadStatus('Failed to upload file');
      }
    } catch (error) {
      setUploadStatus('Error uploading file');
    }
  };

  return (
    <div>
      <h1>Upload CSV File</h1>
      <input type="file" accept=".csv" onChange={handleFileChange} />
      <button onClick={handleFileUpload}>Upload</button>
      <p>{uploadStatus}</p>
    </div>
  );
};

export default UploadCSV;