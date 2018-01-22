using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace GT_LIB_Converter {
    public partial class Form1:Form {
        public Form1() {
            InitializeComponent();
        }

        Bitmap bmp;
        Bitmap pixel;
        Graphics g;
        Thread t;
        delegate void PixelFunc(int x, int y, Color c);
        private void SetPixel(int x, int y, Color c) {
            lock (g)
            {
                if(x>=bmp.Width) return;
                if(y>=bmp.Height) return;
                pixel.SetPixel(0, 0, c);
                g.DrawImageUnscaled(pixel, x, y);
                bmp.SetPixel(x, y, c);
            }
        }

        public void Set_All_Palette(){
         Make_Pal(32*0   ,32*1   ,0 ,0 ,0 ,0 ,64,0 ); /* Green    */
         Make_Pal(32*1   ,32*2   ,0 ,0 ,0 ,0 ,0 ,64); /* Blue     */
         Make_Pal(32*2   ,32*3   ,0 ,0 ,0 ,64,0 ,0 ); /* Red      */
         Make_Pal(32*3   ,32*4   ,0 ,0 ,0 ,64,64,64); /* Gray     */
         Make_Pal(32*4   ,32*5   ,0 ,0 ,0 ,64,64,0 ); /* Gold     */
         Make_Pal(32*5   ,32*6   ,0 ,0 ,0 ,64,0 ,64); /* Purple   */
         Make_Pal(32*6   ,32*6+7 ,0 ,0 ,0 ,13,5 ,0 ); /* Brown » */
         Make_Pal(32*6+7 ,32*7   ,13,5 ,0 ,64,56,49); /* Brown ¼ */
         Make_Pal(32*7   ,32*8   ,0 ,0 ,0 ,32,32,64); /* Cyan     */
        }

        void Make_Pal(int a,int b,int r1,int g1,int b1,int r2,int g2,int b2) {
         int e,f,g,p,i;
         for(i=a;i<b;i++) {
          p=((i-a)*100)/(b-a);
          e=((r2-r1)*p)/100;
          f=((g2-g1)*p)/100;
          g=((b2-b1)*p)/100;
          Pal[i*3+0]=r1+e;
          Pal[i*3+1]=g1+f;
          Pal[i*3+2]=b1+g;
         }
   
        }
        int[] Pal=new int[768];

        private void Form1_Load(object sender,EventArgs e) {
            Set_All_Palette();

            if (g!=null) g.Dispose();
            pictureBox1.Image = null;
            if (bmp != null) bmp.Dispose();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            if (pixel != null) pixel.Dispose();
            pixel = new Bitmap(1, 1);
            pictureBox1.Image = bmp;
            g = pictureBox1.CreateGraphics();
            int x=0,y=0,i=0;
            PixelFunc func = new PixelFunc(SetPixel);
            int pos = 0;

            Bitmap lib=new Bitmap(@"Z:\DM\GE\LIB\GTS.BMP");
            
	        
            
           

            using(BinaryWriter binWriter =new BinaryWriter(File.Open(@"Z:\DM\GE\LIB\TEST.LIB", FileMode.Create))){
            for(i=0;i<63;i++){
                for(y=0;y<32;y++){
                for(x=0;x<32;x++){
                        Color c=lib.GetPixel(x+(i%8)*40, y+((int)(i/8))*40);
                        
                        byte d=0;
                        double distance=0,od=10000;
                        for(int f=0;f<256;f++){
                            int R=(Pal[f*3+0]*4-c.R);
                            int G=(Pal[f*3+1]*4-c.G);
                            int B=(Pal[f*3+2]*4-c.B);
                            distance=Math.Sqrt(R*R+G*G+B*B);
                            if(distance<od) {
                                d=(byte)f;
                                od=distance;
                            }

                        }
                        
                        binWriter.Write(d);
                        }//y
                   
                    }//x
                }//i
                binWriter.Close();
            }
                    

            i=0;
            x=0;
            y=0;
            using (BinaryReader b = new BinaryReader(File.Open(@"Z:\DM\GE\LIB\TEST.LIB", FileMode.Open)))
	        {
	            int length = (int)b.BaseStream.Length;
	            while (pos < length){
		        try{
                    int v =b.ReadByte();
                
        
                        int a3 = 0xFF ;
                        int r3 = Pal[v*3]*4 ;  
                        int g3 = Pal[v*3+1]*4;
                        int b3 = Pal[v*3+2]*4; 
                        
                        Color c=Color.FromArgb(a3,r3,g3,b3);
                    
                        pictureBox1.Invoke(func, x+(i%8)*40, y+((int)(i/8))*40,c);
                   
                    
                    x++;
                    if(x>31) { x=0; y++; }
                    if(y>31) { y=0; i++; }
                    



                    pos ++;
                } catch (Exception je){
                   // MessageBox.Show(je.Message);
                }
	            }
	        }
            //pictureBox1.Image.Save(@"Z:\DM\GE\LIB\GTS.BMP");*/

        }
    }
}
