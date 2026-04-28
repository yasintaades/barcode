"use client";

import { useEffect, useState } from 'react';
import { 
  FileText, RefreshCcw, CheckCircle2, Clock, 
  ArrowRight, FileCode, AlertCircle, X, Printer 
} from 'lucide-react';
import { useRouter } from 'next/navigation';

export default function Dashboard() {
  const router = useRouter();
  const [logs, setLogs] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchLogs = async () => {
    setLoading(true);
    try {
      const res = await fetch('http://localhost:5103/barcode/available-files');
      if (!res.ok) throw new Error("Gagal mengambil daftar log");
      const data = await res.json();
      // Kita tidak lagi memfilter "READY" agar status "printed" juga muncul
      setLogs(data);
    } catch (err) {
      console.error("Gagal fetch data:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchLogs();
  }, []);

  return (
    <div className="max-w-6xl mx-auto p-6">
      {/* Header & Stats */}
      <div className="flex justify-between items-center mb-10">
        <div>
          <h1 className="text-3xl font-extrabold text-slate-900 tracking-tight">Barcode Management</h1>
          <p className="text-slate-500 mt-1">Monitoring SFTP Sync & Printing Status</p>
        </div>
        <button onClick={fetchLogs} className="p-2 bg-white border rounded-full hover:bg-slate-50 shadow-sm">
          <RefreshCcw size={20} className={loading ? "animate-spin text-blue-500" : "text-slate-600"} />
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-10">
        <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200">
          <p className="text-slate-500 text-sm font-medium">New Files</p>
          <h3 className="text-3xl font-bold text-blue-600 mt-1">
            {logs.filter(l => l.statusPrint === 'new').length}
          </h3>
        </div>
        <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200">
          <p className="text-slate-500 text-sm font-medium">Already Printed</p>
          <h3 className="text-3xl font-bold text-emerald-600 mt-1">
            {logs.filter(l => l.statusPrint === 'printed').length}
          </h3>
        </div>
        <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200">
          <p className="text-slate-500 text-sm font-medium">System Source</p>
          <h3 className="text-lg font-bold text-slate-900 mt-1">SFTP Delami</h3>
        </div>
      </div>

      {/* Table */}
      <div className="bg-white rounded-2xl shadow-md border border-slate-200 overflow-hidden">
        <table className="w-full text-left border-collapse">
          <thead>
            <tr className="bg-slate-50 border-b border-slate-200">
              <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase">File Details</th>
              <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase text-center">Sync Date</th>
              <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase text-center">Print Status</th>
              <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase text-right">Action</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100">
            {loading ? (
              <tr><td colSpan={4} className="py-20 text-center"><RefreshCcw className="animate-spin mx-auto text-blue-500" /></td></tr>
            ) : logs.map((log) => (
              <tr key={log.fileName} className="hover:bg-slate-50/50 transition-colors">
                <td className="px-6 py-4">
                  <div className="flex items-center gap-4">
                    <div className={`p-2 rounded-lg ${log.statusPrint === 'new' ? 'bg-blue-50' : 'bg-slate-100'}`}>
                      <FileCode className={log.statusPrint === 'new' ? 'text-blue-600' : 'text-slate-400'} size={24} />
                    </div>
                    <div>
                      <p className="font-bold text-slate-800">{log.fileName}</p>
                      <p className="text-xs text-slate-400">ID: {log.id || 'Pending'}</p>
                    </div>
                  </div>
                </td>
                <td className="px-6 py-4 text-center text-sm text-slate-500">
                  {new Date(log.syncDate).toLocaleString('id-ID')}
                </td>
                <td className="px-6 py-4 text-center">
                  <span className={`px-3 py-1 rounded-full text-[10px] font-extrabold uppercase tracking-widest
                    ${log.statusPrint === 'new' ? 'bg-blue-100 text-blue-700' : 
                      log.statusPrint === 'printed' ? 'bg-emerald-100 text-emerald-700' : 
                      'bg-purple-100 text-purple-700'}`}>
                    {log.statusPrint}
                  </span>
                </td>
                <td className="px-6 py-4 text-right">
                  <button 
                    onClick={() => router.push(`/printing?file=${log.fileName}`)}
                    className="inline-flex items-center gap-2 bg-slate-900 text-white px-4 py-2 rounded-lg text-sm font-bold hover:bg-blue-600 transition-all shadow-sm"
                  >
                    Details <ArrowRight size={16} />
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}