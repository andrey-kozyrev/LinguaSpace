using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Net;
using System.Net.Sockets;

using LinguaSpace.Common;
using LinguaSpace.Common.Resources;
using LinguaSpace.Common.Data;
using LinguaSpace.Words.Practice;
using LinguaSpace.Sync;

namespace LinguaSpace.Words.UI
{
    public partial class MainForm : Form
    {
        private IFlashCardsGenerator _generator;
        private IFlashCard _card;
		private SyncClient _sync;

        int _captionHeight;
        int _textHeight;
        int _offset;

        Brush _brushBk;
        Pen _penBk;

        Bitmap _bmpStatus;

        Bitmap _bmpError;
        Bitmap _bmpInfo;
        Bitmap _bmpWarning;
        Bitmap _bmpOK;

        Color _colorCaption;
        Color _colorText;
        Color _colorRight;
        Color _colorWrong;

        Font _fontCaption;
        Font _fontTextWord;
        Font _fontTextTranslations;
        Font _fontTextDefinition;
        Font _fontTextExample;
        Font _fontStatus;

        Label _lblQuestionCaption;
        Label _lblQuestion;
        Label _lblAnswersCaption;
        Panel _pnlAnswers;
        ListView _lstAnswers;
        Label _lblExampleCaption;
        Label _lblExample;
        Label _lblStatus;

        public MainForm()
        {
            InitializeComponent();
            BuildUI();
			_sync = new SyncClient(FileUtils.VocabulariesFolder, "*.lsv");
			this.Load += new EventHandler(MainForm_Load);
            this.Closing += new CancelEventHandler(MainForm_Closing);
        }

        private void MainForm_Closing(Object sender, CancelEventArgs e)
        {
            _generator.Dispose();
			_sync.Shutdown();
        }

		private void MainForm_Load(Object sender, EventArgs e)
		{
			ScanVocabularies();
			CreateGenerator();
			DoNext();
			_sync.StartUp();
		}

        private void BuildUI()
        {
            this.SuspendLayout();

            this._captionHeight = 16;
            this._textHeight = 22;
            this._offset = 7;

            this._brushBk = new SolidBrush(SystemColors.Control);
            this._penBk = new Pen(SystemColors.Control, 1);

            Assembly assembly = Assembly.GetExecutingAssembly();
			this._bmpError = new Bitmap(assembly.GetManifestResourceStream("LinguaSpace.Words.Resources.Icons.Status.Error.png"));
			this._bmpWarning = new Bitmap(assembly.GetManifestResourceStream("LinguaSpace.Words.Resources.Icons.Status.Warning.png"));
			this._bmpInfo = new Bitmap(assembly.GetManifestResourceStream("LinguaSpace.Words.Resources.Icons.Status.Info.png"));
			this._bmpOK = new Bitmap(assembly.GetManifestResourceStream("LinguaSpace.Words.Resources.Icons.Status.OK.png"));
            this._bmpStatus = this._bmpInfo;

            this._fontCaption = new Font("Nina", 8, FontStyle.Regular);
            this._fontTextWord = new Font("Segoe Condensed", 10, FontStyle.Bold);
            this._fontTextTranslations = new Font("Segoe Condensed", 10, FontStyle.Bold);
            this._fontTextDefinition = new Font("Segoe Condensed", 10, FontStyle.Bold);
            this._fontTextExample = new Font("Segoe Condensed", 8, FontStyle.Bold);
            this._fontStatus = new Font("Nina", 8, FontStyle.Regular);

            this._colorText = SystemColors.ControlText;
            this._colorCaption = SystemColors.ControlText;
            this._colorRight = Color.Green;
            this._colorWrong = Color.Red;

            this._lblQuestionCaption = new Label();
            this._lblQuestionCaption.Text = "Question:";
            this._lblQuestionCaption.Font = this._fontCaption;
            this._lblQuestionCaption.ForeColor = this._colorCaption;
            this.Controls.Add(this._lblQuestionCaption);

            this._lblQuestion = new Label();
            this.Controls.Add(this._lblQuestion);

            this._lblAnswersCaption = new Label();
            this._lblAnswersCaption.Text = "Answers:";
            this._lblAnswersCaption.Font = this._fontCaption;
            this._lblAnswersCaption.ForeColor = this._colorCaption;
            this.Controls.Add(this._lblAnswersCaption);

            this._pnlAnswers = new Panel();
            this.Controls.Add(this._pnlAnswers);

            this._lstAnswers = new ListView();
            this._lstAnswers.View = View.Details;
            this._lstAnswers.FullRowSelect = true;
            this._lstAnswers.HeaderStyle = ColumnHeaderStyle.None;
            this._lstAnswers.Clear();
            this._lstAnswers.Columns.Add(String.Empty, this._lstAnswers.Width - 2, HorizontalAlignment.Left);
            this._pnlAnswers.Controls.Add(this._lstAnswers);

            this._lblExampleCaption = new Label();
            this._lblExampleCaption.ForeColor = this._colorCaption;
            this._lblExampleCaption.Font = this._fontCaption;
            this._lblExampleCaption.Text = "Example:";
            this.Controls.Add(this._lblExampleCaption);

            this._lblExample = new Label();
            this._lblExample.Font = this._fontTextExample;
            this.Controls.Add(this._lblExample);

            this._lblStatus = new Label();
            this._lblStatus.Font = this._fontStatus;
            this._lblStatus.BackColor = SystemColors.Control;
            this.Controls.Add(this._lblStatus);

            this.Resize += new EventHandler(MainForm_Resize);

            this.ResumeLayout(true);
        }

        private void ScanVocabularies()
        {
            this.menuItemRight.MenuItems.Clear();

            DirectoryInfo folder = new DirectoryInfo(FileUtils.VocabulariesFolder);
            FileInfo[] files = folder.GetFiles("*.lsv");

            if (files.Length > 0)
            {
                int count = 0;
                DateTime dt = DateTime.MinValue;
                foreach (FileInfo file in files)
                {
                    if (file.LastAccessTime > dt)
                    {
                        dt = file.LastAccessTime;
                    }

                    MenuItem item = new MenuItem();
                    item.Text = file.Name;
                    item.Click += new EventHandler(VocabularyItem_Click);
                    this.menuItemRight.MenuItems.Add(item);
                }

                MenuItem separator = new MenuItem();
                separator.Text = "-";
                this.menuItemRight.MenuItems.Add(separator);
            }

            this.menuItemRight.MenuItems.Add(this.menuItemSync);
        }

        private void CreateGenerator()
        {
			String vocabulary = Path.Combine(FileUtils.VocabulariesFolder, "German-Russian.lsv");
			String profile = Path.Combine(FileUtils.ProfilesFolder, "Andrey.lsp");
			_generator = new FlashCardsGenerator(vocabulary, profile);
        }

        private Font GetFont(FlashCardItemType type)
        {
            Font font = this.Font;
            switch (type)
            {
                case FlashCardItemType.Word:
                    font = this._fontTextWord;
                    break;
                case FlashCardItemType.Translations:
                case FlashCardItemType.Synonyms:
                case FlashCardItemType.Antonyms:
                    font = this._fontTextTranslations;
                    break;
                case FlashCardItemType.Definition:
                    font = this._fontTextDefinition;
                    break;
            }
            return font;
        }

        private void DoNext()
        {
            Debug.Assert(_generator != null);

            _bmpStatus = _bmpInfo;
            _lblStatus.Text = "Generating card...";
            Refresh();

            _card = _generator.Generate();

            _lblQuestionCaption.Text = _card.Question.Caption + ":";
			_lblQuestion.Text = _card.Question.Text;
			_lblQuestion.ForeColor = _colorText;
			_lblQuestion.Font = GetFont(_card.Question.Type);

			_lstAnswers.Items.Clear();
			foreach (IFlashCardItem cardItem in _card.Answers)
            {
				_lblAnswersCaption.Text = cardItem.Caption + ":";
				_lstAnswers.Font = GetFont(cardItem.Type);
                ListViewItem item = new ListViewItem();
                item.Text = cardItem.Text;
                item.Tag = cardItem;
                item.ForeColor = SystemColors.ControlText;
				_lstAnswers.Items.Add(item);
            }
			FormsUtils.SelectItem(_lstAnswers, 0);

            _lblQuestionCaption.ForeColor = (_lblAnswersCaption.Text == "Antonyms:") ? Color .Red : SystemColors.WindowText;
            _lblAnswersCaption.ForeColor = _lblQuestionCaption.ForeColor;

			_lblExample.Text = String.Empty;

			_bmpStatus = _bmpInfo;
			_lblStatus.Text = "Please select answer";

			menuItemLeft.Text = "Answer";

			Refresh();
        }

        private void DoAnswer()
        {
            Debug.Assert(_card != null);

            ListViewItem itemAnswer = FormsUtils.GetSelectedItem(_lstAnswers);
            Debug.Assert(itemAnswer != null);
            IFlashCardItem cardItemAnswer = (IFlashCardItem)itemAnswer.Tag;

            _card.Answer(cardItemAnswer);

            _lblQuestion.ForeColor = _colorRight;

            foreach (ListViewItem item in _lstAnswers.Items)
            {
                item.Selected = false;
                item.Focused = false;
                if (item.Tag.Equals(_card.Question))
                {
                    item.ForeColor = _colorRight;
                }
                else if (item.Tag == itemAnswer.Tag)
                {
                    item.ForeColor = _colorWrong;
                }
            }

            switch (_card.Status)
            {
                case FlashCardStatus.Right:
                    _bmpStatus = _bmpOK;
                    _lblStatus.Text = "Your answer is right";
                    break;
                case FlashCardStatus.Prompt:
                    _bmpStatus = _bmpWarning;
                    _lblStatus.Text = "Here is the right answer";
                    break;
                case FlashCardStatus.Wrong:
                    _bmpStatus = _bmpError;
                    _lblStatus.Text = "Your answer is wrong";
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            _lblExample.Text = _card.Example.Replace("[", "").Replace("]", "");

            menuItemLeft.Text = "Next";

            Refresh();
        }

        private void DoNextOrAnswer()
        {
            Debug.Assert(this._card != null);
            if (this._card.Status == FlashCardStatus.Question)
            {
                DoAnswer();
            }
            else
            {
                DoNext();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.Enter:
                    DoNextOrAnswer();
                    break;
                default:
                    break;
            }
        }

        private void MainForm_Resize(Object sender, EventArgs e)
        {
            Size sizeForm = this.ClientRectangle.Size;

            this._lblQuestionCaption.Bounds = new Rectangle(0, 0, sizeForm.Width, this._captionHeight);
            this._lblQuestion.Bounds = new Rectangle(2, this._lblQuestionCaption.Bounds.Bottom + 1, sizeForm.Width - 2, this._textHeight);
            this._lblAnswersCaption.Bounds = new Rectangle(0, this._lblQuestion.Bounds.Bottom + this._offset, sizeForm.Width, this._captionHeight);
            this._pnlAnswers.Bounds = new Rectangle(0, this._lblAnswersCaption.Bounds.Bottom + 1, sizeForm.Width, 90);
            this._lblExampleCaption.Bounds = new Rectangle(0, this._pnlAnswers.Bounds.Bottom + this._offset, sizeForm.Width, this._captionHeight);

            this._lblExample.Bounds = new Rectangle(2, this._lblExampleCaption.Bounds.Bottom + 1, sizeForm.Width - 2, sizeForm.Height - this._lblExampleCaption.Bounds.Bottom - 20);

            this._lblStatus.Bounds = new Rectangle(20, sizeForm.Height - this._captionHeight - 2, sizeForm.Width-20, this._captionHeight + 2);

            this._lstAnswers.Bounds = new Rectangle(-3, -1, this._pnlAnswers.Width + 4, this._pnlAnswers.Height + 2);
            this._lstAnswers.Columns[0].Width = this._lstAnswers.Width - 2;
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            if (this._card.Status == FlashCardStatus.Question)
            {
                DoAnswer();
            }
            else
            {
                DoNext();
            }
        }

        private void UnderlineLabel(Graphics g, Label label, Color color1, Color color2)
        {
            int x1 = this.ClientRectangle.Left;
            int x2 = this.ClientRectangle.Right;
            int y = label.Bottom;

            int steps = 24;
            for (int i = 0; i < steps; ++i)
            {
                int red = color1.R + (color2.R - color1.R) * (steps - i) / steps;
                int green = color1.G + (color2.G - color1.G) * (steps - i) / steps;
                int blue = color1.B + (color2.B - color1.B) * (steps - i) / steps;
                Color clr = Color.FromArgb(red, green, blue);

                int x1i = x1 + (x2 - x1) * i / steps;
                int x2i = x1 + (x2 - x1) * (i + 1) / steps;
                
                using (Pen p = new Pen(clr))
                {
                    g.DrawLine(p, x1i, y, x2i, y);
                }
            }
        }

        private void MainForm_Paint(Object sender, PaintEventArgs e)
        {
            Rectangle rc = this.ClientRectangle;
            rc.Y = rc.Bottom - this._captionHeight - 4;
            rc = Rectangle.Intersect(rc, e.ClipRectangle);
            e.Graphics.FillRectangle(this._brushBk, rc);

            UnderlineLabel(e.Graphics, this._lblQuestionCaption, SystemColors.Window, SystemColors.Highlight);
            UnderlineLabel(e.Graphics, this._lblAnswersCaption, SystemColors.Window, SystemColors.Highlight);
            UnderlineLabel(e.Graphics, this._lblExampleCaption, SystemColors.Window, SystemColors.Highlight);

            ImageAttributes ia = new ImageAttributes();
            ia.SetColorKey(Color.Transparent, Color.Transparent);
            Rectangle rect = new Rectangle(2, this.ClientRectangle.Bottom - this._captionHeight - 2, this._bmpStatus.Width, this._bmpStatus.Height);
            e.Graphics.DrawImage(this._bmpStatus, rect, 0, 0, this._bmpStatus.Width, this._bmpStatus.Height, GraphicsUnit.Pixel, ia);
        }

        private void VocabularyItem_Click(Object sender, EventArgs e)
        {
        }

        private void menuItemSync_Click(object sender, EventArgs e)
        {

        }
    }
}