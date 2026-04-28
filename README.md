CREATE TABLE barcode_files (
    id SERIAL PRIMARY KEY,
    file_name VARCHAR(255) NOT NULL UNIQUE,
    po_number VARCHAR(100),
    total_article INT DEFAULT 0,
    total_qty INT DEFAULT 0,
    status_print VARCHAR(20) DEFAULT 'new',
    sync_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
