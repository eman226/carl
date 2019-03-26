
import sys
file = open(sys.argv[1], 'r')
import re
import nltk
nltk.download('stopwords')
from nltk.corpus import stopwords 

#text=sys.argv[1]
stop=set(stopwords.words('english'))

document=file.read()
document = re.sub('[^A-Za-z ]+', ' ', document)
document = ' '.join(document.split())
document = ' '.join([i for i in document.split() if i not in stop])
#sprint(document)

nltk.download('punkt')
nltk.download('averaged_perceptron_tagger')
words = nltk.tokenize.word_tokenize(document)
words = [word.lower() for word in words if word not in stop]
#words = [word.lower() for word in words if word not in stop]
tagged = nltk.pos_tag(words)
nouns = [word for word,pos in tagged 
            if (pos == 'NN' or pos == 'NNP' or pos == 'NNS' or pos == 'NNPS')]
fdist = nltk.FreqDist(nouns)

freq_noun_list = []
for w, c in fdist.most_common(5) :
  freq_noun_list.append(w)
print(freq_noun_list)
#print(document)
