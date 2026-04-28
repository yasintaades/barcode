"use client";
import { useSearchParams, useRouter } from 'next/navigation';
import { Printer, ArrowLeft } from 'lucide-react';

export default function PrintingPage() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const fileName = searchParams.get('file');

  return (
    <div className="p-8 max-w-6xl mx-auto">
      <button onClick={() => router.back()} className="flex items-center gap-2 text-slate-500 hover:text-slate-800 mb-6 transition-colors">
        <ArrowLeft size={20} /> Back to Dashboard
      </button>

      <div className="bg-white p-8 rounded-3xl shadow-xl border border-slate-100">
        <div className="flex justify-between items-start mb-8">
          <div>
            <h1 className="text-2xl font-black text-slate-900">Printing Details</h1>
            <p className="text-slate-400 font-mono text-sm">{fileName}</p>
          </div>
          <div className="bg-blue-50 text-blue-700 px-4 py-2 rounded-xl text-sm font-bold">
            PO Source: Cegid
          </div>
        </div>

        <div className="overflow-hidden rounded-2xl border border-slate-100">
          <table className="w-full text-left">
            <thead className="bg-slate-50 text-slate-500 text-xs uppercase font-bold">
              <tr>
                <th className="p-4">No</th>
                <th className="p-4">No PO</th>
                <th className="p-4">Sync Date</th>
                <th className="p-4 text-center">Total Article</th>
                <th className="p-4 text-center">Total Qty</th>
                <th className="p-4 text-right">Action</th>
              </tr>
            </thead>
            <tbody className="text-sm font-medium">
              <tr className="border-t hover:bg-slate-50/50">
                <td className="p-4">1</td>
                <td className="p-4 text-blue-600 font-bold">PO-2026-00421</td>
                <td className="p-4 text-slate-400">28/04/2026</td>
                <td className="p-4 text-center">12</td>
                <td className="p-4 text-center">480</td>
                <td className="p-4 text-right">
                  <button className="inline-flex items-center gap-2 bg-blue-600 text-white px-5 py-2.5 rounded-xl font-bold hover:bg-blue-700 transition-all">
                    <Printer size={18} /> Print Barcode
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}