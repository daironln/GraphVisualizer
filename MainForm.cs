using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using GraphVisualizerForm.Mathematics;

namespace GraphVisualizerForm;
public partial class MainForm : Form
{
    private int[,] matrizAdyacencia = new int[,] 
    {

        {1, 0, 1, 0, 0, 0, 0, 0, 1, 0},
        {1, 0, 0, 1, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 1, 1, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
        {0, 1, 0, 0, 0, 1, 0, 0, 0, 0},
        {1, 0, 0, 0, 1, 1, 0, 0, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {0, 1, 0, 0, 0, 0, 1, 0, 0, 0},
        {0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 1, 0, 1, 0, 1}

    };

    public MainForm()
    {
        InitializeComponent();
            
        this.Paint += new PaintEventHandler(GrafoPaintForm);
    }

    private void GrafoPaintForm(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        DibujarGrafo(e.Graphics);
    }

    private void DibujarGrafo(Graphics g)
    {
        int nodos = matrizAdyacencia.GetLength(0);
        PointF[] posiciones = CalcularPosicionesNodos();
        float diametroNodo = 30;

        using (Pen penArista = new Pen(Color.Gray, 2))
        {
            for (int i = 0; i < nodos; i++)
            {
                for (int j = 0; j < nodos; j++)
                {
                    if (matrizAdyacencia[i, j] == 1)
                    {
                        DibujarFlecha(g, penArista, posiciones[i], posiciones[j], diametroNodo, i == j);
                    }
                }
            }
        }

        using (Brush brushNodo = new SolidBrush(Color.LightBlue))
        using (Pen penNodo = new Pen(Color.DarkBlue, 2))
        {
            for (int i = 0; i < nodos; i++)
            {
                RectangleF rect = new RectangleF(
                        posiciones[i].X - diametroNodo / 2,
                        posiciones[i].Y - diametroNodo / 2,
                        diametroNodo,
                        diametroNodo);
                g.FillEllipse(brushNodo, rect);
                g.DrawEllipse(penNodo, rect);

                using (Font font = new Font("Arial", 10))
                using (Brush brushTexto = new SolidBrush(Color.Black)) 
                {
                    string texto = i.ToString();
                    SizeF textSize = g.MeasureString(texto, font);
                    g.DrawString(texto, font, brushTexto,
                            posiciones[i].X - textSize.Width / 2,
                            posiciones[i].Y - textSize.Height / 2);
                }
            }
        }
    }

    private void DibujarFlecha(Graphics g, Pen pen, PointF origen, PointF destino, float diametroNodo, bool esBucle)
    {
        // Conf flecha
        float tamañoFlecha = 12;
        float anguloFlecha = 30 * (float)Math.PI / 180;
            
        if (esBucle)
        {
            DibujarBucle(g, pen, origen);
            return;
        }

        Vector2 direccion = new Vector2(destino.X - origen.X, destino.Y - origen.Y);
        direccion = direccion.Normalize();
        float radio = diametroNodo / 2;
        PointF puntoFinal = new PointF(
                destino.X - direccion.X * radio,
                destino.Y - direccion.Y * radio);

        g.DrawLine(pen, origen, puntoFinal);


        PointF[] puntosFlecha = new PointF[3];
        puntosFlecha[0] = puntoFinal;

        // Rot izquierda
        double anguloRad = anguloFlecha;
        Vector2 izquierda = new Vector2(
                (float)(direccion.X * Math.Cos(anguloRad) - direccion.Y * Math.Sin(anguloRad)),
                (float)(direccion.X * Math.Sin(anguloRad) + direccion.Y * Math.Cos(anguloRad))) * tamañoFlecha;

        // Rot derecha
        anguloRad = -anguloRad;
        Vector2 derecha = new Vector2(
                (float)(direccion.X * Math.Cos(anguloRad) - direccion.Y * Math.Sin(anguloRad)),
                (float)(direccion.X * Math.Sin(anguloRad) + direccion.Y * Math.Cos(anguloRad))) * tamañoFlecha;

        puntosFlecha[1] = new PointF(
                puntoFinal.X - izquierda.X,
                puntoFinal.Y - izquierda.Y);

        puntosFlecha[2] = new PointF(
                puntoFinal.X - derecha.X,
                puntoFinal.Y - derecha.Y);

        g.FillPolygon(Brushes.Gray, puntosFlecha);
    }

    private void DibujarBucle(Graphics g, Pen pen, PointF nodo)
    {
        float radioBucle = 20f;
        float tamañoFlecha = 10f;
        float anguloFlecha = 30 * (float)Math.PI / 180;

        // Dibujar bucle
        RectangleF rectBucle = new RectangleF(
                nodo.X - radioBucle,
                nodo.Y - radioBucle,
                radioBucle * 2,
                radioBucle * 2);
        g.DrawArc(pen, rectBucle, 180, 270);
        
        // Calcular pos flecha
        float anguloFinal = (180 + 270) * (float)Math.PI / 180;
        PointF puntoFlecha = new PointF(
                nodo.X + radioBucle * (float)Math.Cos(anguloFinal),
                nodo.Y + radioBucle * (float)Math.Sin(anguloFinal));
        
        // Calcular dir flecha
        Vector2 direccion = new Vector2(
                -(float)Math.Sin(anguloFinal),
                (float)Math.Cos(anguloFinal)).Normalize();

        // Dibujar flecha
        PointF[] puntosFlecha = new PointF[3];
        puntosFlecha[0] = puntoFlecha;

        Vector2 izquierda = new Vector2(
                (float)(direccion.X * Math.Cos(anguloFlecha) - direccion.Y * Math.Sin(anguloFlecha)),
                (float)(direccion.X * Math.Sin(anguloFlecha) + direccion.Y * Math.Cos(anguloFlecha)))
                * tamañoFlecha;

        Vector2 derecha = new Vector2(
                (float)(direccion.X * Math.Cos(-anguloFlecha) - direccion.Y * Math.Sin(-anguloFlecha)),
                (float)(direccion.X * Math.Sin(-anguloFlecha) + direccion.Y * Math.Cos(-anguloFlecha)))
                * tamañoFlecha;
        
        
        puntosFlecha[1] = new PointF(
                puntoFlecha.X - izquierda.X,
                puntoFlecha.Y - izquierda.Y);

        puntosFlecha[2] = new PointF(
                puntoFlecha.X - derecha.X,
                puntoFlecha.Y - derecha.Y);

        g.FillPolygon(Brushes.Gray, puntosFlecha);
    }

    private PointF[] CalcularPosicionesNodos()
    {
        int nodos = matrizAdyacencia.GetLength(0);
        PointF[] posiciones = new PointF[nodos];

        Rectangle area = this.ClientRectangle;
        float radio = Math.Min(area.Width, area.Height) * 0.4f;
        PointF centro = new PointF(area.Width / 2, area.Height / 2);

        for (int i = 0; i < nodos; i++)
        {
            double angulo = 2 * Math.PI * i / nodos;
            posiciones[i] = new PointF(
                    centro.X + radio * (float)Math.Cos(angulo),
                    centro.Y + radio * (float)Math.Sin(angulo));
        }

        return posiciones;
    }
}